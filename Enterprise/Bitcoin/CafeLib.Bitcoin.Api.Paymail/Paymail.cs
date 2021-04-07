using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.Paymail.Models;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Script;
using CafeLib.Bitcoin.Utility;
using CafeLib.Core.Extensions;
using CafeLib.Web.Request;
using DnsClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CafeLib.Bitcoin.Api.Paymail
{
    public class Paymail : BasicApiRequest
    {
        private const string HandleRegexPattern = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        private static readonly Lazy<Regex> HandleRegex = new Lazy<Regex>(() => new Regex(HandleRegexPattern), true);

        private readonly IDictionary<string, CapabilitiesResponse> _cache;

        /// <summary>
        /// Paymail api default constructor.
        /// </summary>
        public Paymail()
        {
             _cache = new ConcurrentDictionary<string, CapabilitiesResponse>();
             Headers.Add("User-Agent", "KzPaymailClient");
             Headers.Add("Accept", WebContentType.Json);
        }

        /// <summary>
        /// Determine whether domain has capability.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="capability"></param>
        /// <returns></returns>
        public async Task<bool> DomainHasCapability(string domain, Capability capability)
        {
            var id = ToBrfcId(capability);
            var ba = await GetApiDescriptionFor(domain);
            if (ba == null || !ba.Capabilities.ContainsKey(id))
                return false;
            var v = ba.Capabilities[id].Value;
            return v != null && !v.Equals(false);
        }

        /// <summary>
        /// Ensure capability
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="capability"></param>
        /// <returns></returns>
        public async Task EnsureCapability(string domain, Capability capability)
        {
            if (!await DomainHasCapability(domain, capability))
                throw new InvalidOperationException($"Unknown capability \"{capability}\" for \"{domain}\"");
        }

        /// <summary>
        /// Get public key.
        /// </summary>
        /// <param name="receiverHandle"></param>
        /// <returns></returns>
        public async Task<KzPubKey> GetPubKey(string receiverHandle)
        {
            var url = await GetIdentityUrl(receiverHandle);
            var json = await GetAsync(url);
            var response = JsonConvert.DeserializeObject<GetPubKeyResponse>(json);
            var pubkey = new KzPubKey(response.PubKey);
            return pubkey.IsCompressed && new[] { 2, 3 }.ToArray().Contains(pubkey.ReadOnlySpan[0])
                ? pubkey
                : null;
        }

        /// <summary>
        /// Verify public key.
        /// </summary>
        /// <param name="receiverHandle"></param>
        /// <param name="pubKey"></param>
        /// <returns></returns>
        public async Task<bool> VerifyPubKey(string receiverHandle, KzPubKey pubKey)
        {
            var url = await GetVerifyUrl(receiverHandle, pubKey.ToHex());

            var json = await GetAsync(url);
            var response = JsonConvert.DeserializeObject<VerifyPubKeyResponse>(json);
            return response.PubKey == pubKey.ToHex() && response.Match;
        }

        /// <summary>
        /// Implements brfc 759684b1a19a, paymentDestination: bsvalias Payment Addressing (Basic Address Resolution)
        /// </summary>
        /// <param name="key">Private key with which to sign this request. If null, signature will be blank. Else, must match public key returned by GetPubKey(senderHandle).</param>
        /// <param name="receiverHandle"></param>
        /// <param name="senderHandle"></param>
        /// <param name="senderName"></param>
        /// <param name="amount"></param>
        /// <param name="purpose"></param>
        /// <returns></returns>
        public async Task<KzScript> GetOutputScript(KzPrivKey key, string receiverHandle, string senderHandle, string senderName = null, KzAmount? amount = null, string purpose = "")
        {
            amount ??= KzAmount.Zero;
            var dt = DateTime.UtcNow.ToString("o");
            var message = $"{senderHandle}{amount.Value.Satoshis}{dt}{purpose}";
            var signature = key?.SignMessageToB64(message) ?? "";

            // var ok = key.GetPubKey().VerifyMessage(message, signature);

            var request = new GetOutputScriptRequest
            {
                SenderHandle = senderHandle,
                Amount = amount.Value.Satoshis,
                Timestamp = dt,
                Purpose = purpose ?? "",
                SenderName = senderName ?? "",
                Signature = signature
            };

            var url = await GetAddressUrl(receiverHandle);
            var json = JObject.FromObject(request);

            var response = await PostAsync(url, json);
            // e.g. {"output":"76a914bdfbe8a16162ba467746e382a081a1857831811088ac"} 
            var outputScript = JsonConvert.DeserializeObject<GetOutputScriptResponse>(response);
            return new KzScript(outputScript.Output);
        }

        /// <summary>
        /// Verifies that the message was signed by the private key corresponding to the paymail public key.
        /// </summary>
        /// <param name="message">A copy of the message which was originally signed.</param>
        /// <param name="signature">The signature received for validation.</param>
        /// <param name="paymail">The paymail claiming to have signed the message.</param>
        /// <param name="pubkey">If known, the public key corresponding to the private key used by the paymail to sign messages.</param>
        /// <returns>(ok, pubkey) where ok is true only if both the public key and signature were confirmed as valid.
        /// If ok is true, the returned public key is valid and can be saved for future validations.
        /// </returns>
        public async Task<(bool ok, KzPubKey pubkey)> IsValidSignature(string message, string signature, string paymail, KzPubKey pubkey = null)
        {
            if (!TryParse(paymail, out _, out var domain)) return (false, pubkey);

            if (pubkey != null)
            {
                // If a pubkey is provided and the domain is capable, verify that it is correct
                // If it is not correct, forget the input value and attempt to obtain the valid key.
                if (await DomainHasCapability(domain, Capability.VerifyPublicKeyOwner))
                {
                    if (!await VerifyPubKey(paymail, pubkey))
                        pubkey = null;
                }
            }

            if (pubkey == null)
            {
                // Attempt to determine the correct pubkey for the paymail.
                if (await DomainHasCapability(domain, Capability.Pki))
                {
                    pubkey = await GetPubKey(paymail);
                }
            }

            return pubkey != null 
                ? (pubkey.VerifyMessage(message, signature), pubkey)
                : (false, null);
        }

        #region Helpers

        private async Task<string> GetIdentityUrl(string paymail) => await GetCapabilityUrl(Capability.Pki, paymail);
        private async Task<string> GetAddressUrl(string paymail) => await GetCapabilityUrl(Capability.PaymentDestination, paymail);
        private async Task<string> GetVerifyUrl(string paymail, string pubkey) => await GetCapabilityUrl(Capability.VerifyPublicKeyOwner, paymail, pubkey);

        /// <summary>
        /// BRFC identifiers are partially defined here: http://bsvalias.org
        /// </summary>
        private static string ToBrfcId(Capability capability)
        {
            return capability.GetDescriptor();
        }

        private async Task<CapabilitiesResponse> GetApiDescriptionFor(string domain, bool ignoreCache = false)
        {
            if (!ignoreCache && _cache.TryGetValue(domain, out var ba))
                return ba;

            var hostname = domain;
            var dns = new LookupClient();
            var r2 = await dns.QueryAsync($"_bsvalias._tcp.{domain}", QueryType.SRV);
            if (!r2.HasError && r2.Answers.Count == 1)
            {
                var srv = r2.Answers[0] as DnsClient.Protocol.SrvRecord;
                hostname = $"{srv?.Target.Value[..^1]}:{srv?.Port}";
            }

            try
            {
                var json = await GetAsync($"https://{hostname}/.well-known/bsvalias");
                var capabilities = JsonConvert.DeserializeObject<CapabilitiesResponse>(json);
                _cache[domain] = capabilities;
                return capabilities;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> GetCapabilityUrl(Capability capability, string paymail, string pubkey = null)
        {
            if (!TryParse(paymail, out var alias, out var domain)) return null;

            await EnsureCapability(domain, capability);
            var ba = await GetApiDescriptionFor(domain);
            var url = ba.Capabilities[ToBrfcId(capability)].Value<string>();
            url = url.Replace("{alias}", alias).Replace("{domain.tld}", domain);
            if (pubkey != null)
                url = url.Replace("{pubkey}", pubkey);
            return url;
        }

        internal static bool TryParse(string paymail, out string alias, out string domain)
        {
            alias = null;
            domain = null;

            if (!HandleRegex.Value.IsMatch(paymail)) return false;

            var parts = paymail.Split('@');
            alias = parts[0];
            domain = parts[1];
            return true;
        }

        #endregion
    }
}

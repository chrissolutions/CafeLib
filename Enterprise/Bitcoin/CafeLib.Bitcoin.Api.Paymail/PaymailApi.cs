using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.Paymail.Models;
using CafeLib.Core.Extensions;
using CafeLib.Web.Request;
using DnsClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CafeLib.Bitcoin.Api.Paymail
{
    public class PaymailApi : BasicApiRequest
    {
        private const string HandleRegexPattern = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        private static readonly Lazy<Regex> HandleRegex = new Lazy<Regex>(() => new Regex(HandleRegexPattern), true);

        private readonly IDictionary<string, CapabilitiesResponse> _cache;

        public PaymailApi()
        {
             _cache = new ConcurrentDictionary<string, CapabilitiesResponse>();
             Headers.Add("User-Agent", "KzPaymailClient");
        }

        public async Task<bool> DomainHasCapability(string domain, Capability capability)
        {
            var id = ToBrfcId(capability);
            var ba = await GetApiDescriptionFor(domain);
            if (ba == null || !ba.Capabilities.ContainsKey(id))
                return false;
            var v = ba.Capabilities[id].Value;
            return !v.Equals(false);
        }

        public async Task EnsureCapability(string domain, Capability capability)
        {
            if (!await DomainHasCapability(domain, capability))
                throw new InvalidOperationException($"Unknown capability \"{capability}\" for \"{domain}\"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ba"></param>
        private void CacheUpdateValue(string domain, CapabilitiesResponse ba)
        {
            _cache[domain] = ba;
        }

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
                hostname = srv?.Target.Value[..^1] + ":" + srv?.Port;
            }

            var json = await GetAsync($"https://{hostname}/.well-known/bsvalias");
            var results = JsonConvert.DeserializeObject<CapabilitiesResponse>(json);
            return results;
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
    }
}

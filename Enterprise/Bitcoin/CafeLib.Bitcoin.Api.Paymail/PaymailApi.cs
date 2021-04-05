using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;
using CafeLib.Web.Request;
using DnsClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CafeLib.Bitcoin.Api.Paymail
{
    public class PaymailApi : ApiRequest<string, JToken>
    {
        private readonly IDictionary<string, CapabilitiesResponse> _cache;

        public PaymailApi()
        {
             _cache = new ConcurrentDictionary<string, CapabilitiesResponse>();
             Headers.Add("User-Agent", "KzPaymailClient");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ba"></param>
        /// <returns></returns>
        private bool CacheTryGetValue(string domain, out CapabilitiesResponse ba)
        {
            return _cache.TryGetValue(domain, out ba);
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
            if (!ignoreCache && CacheTryGetValue(domain, out var ba))
                return ba;

            var hostname = domain;
            var dns = new LookupClient();
            var r2 = await dns.QueryAsync($"_bsvalias._tcp.{domain}", QueryType.SRV);
            if (!r2.HasError && r2.Answers.Count == 1)
            {
                var srv = r2.Answers[0] as DnsClient.Protocol.SrvRecord;
                hostname = srv?.Target.Value[0..^1] + ":" + srv?.Port;
            }

            var json = await GetAsync($"https://{hostname}/.well-known/bsvalias");
            var results = JsonConvert.DeserializeObject<CapabilitiesResponse>(json);
            return results;
        }
    }
}

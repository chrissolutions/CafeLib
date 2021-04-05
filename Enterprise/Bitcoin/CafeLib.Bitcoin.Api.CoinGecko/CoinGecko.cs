using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.CoinGecko.Models;
using CafeLib.Web.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CafeLib.Bitcoin.Api.CoinGecko
{
    public class CoinGecko : ApiRequest<string, JToken>
    {
        private const string CoinGeckoUrl = "https://api.coingecko.com/api/v3";
 
        public CoinGecko()
        {
            Headers.Add("Accept", "application/json");
        }

        public async Task<IEnumerable<SupportedCoin>> GetSupportedCoins()
        {
            var endpoint = $"{CoinGeckoUrl}/coins/list";
            var json = await GetAsync(endpoint);
            return JsonConvert.DeserializeObject<IEnumerable<SupportedCoin>>(json);
        }
    }
}

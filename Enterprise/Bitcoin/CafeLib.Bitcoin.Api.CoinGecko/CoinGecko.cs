﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.CoinGecko.Models.Coins;
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

        public async Task<IEnumerable<Coin>> GetCoinList()
        {
            var endpoint = $"{CoinGeckoUrl}/coins/list";
            var json = await GetAsync(endpoint);
            return JsonConvert.DeserializeObject<IEnumerable<Coin>>(json);
        }

        public async Task<CoinFullData> GetCurrentData(string id)
        {
            var endpoint = $"{CoinGeckoUrl}/coins/{id}";
            var json = await GetAsync(endpoint);
            return JsonConvert.DeserializeObject<CoinFullData>(json);
        }
    }
}

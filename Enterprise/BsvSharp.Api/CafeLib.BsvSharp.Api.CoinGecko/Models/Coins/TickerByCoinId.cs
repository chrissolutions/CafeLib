using CafeLib.Bitcoin.Api.CoinGecko.Models.Shared;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.CoinGecko.Models.Coins
{
    public class TickerById
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tickers")]
        public Ticker[] Tickers { get; set; }
    }
}
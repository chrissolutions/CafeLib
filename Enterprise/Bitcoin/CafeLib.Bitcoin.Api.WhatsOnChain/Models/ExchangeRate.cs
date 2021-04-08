using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain.Models
{
    public class ExchangeRate
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }
    }
}
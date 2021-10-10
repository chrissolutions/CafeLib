using Newtonsoft.Json;

namespace CafeLib.BsvSharp.Mapi.Models
{
    public class ProviderQuote : Envelope
    {
        [JsonProperty("providerName")]
        public string ProviderName { get; set; }

        [JsonProperty("providerId")]
        public string ProviderId { get; set; }

        [JsonProperty("quote")]
        public Quote Quote { get; set; }
    }
}

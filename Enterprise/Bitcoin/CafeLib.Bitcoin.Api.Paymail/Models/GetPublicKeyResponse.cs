using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.Paymail.Models
{
    internal class GetPublicKeyResponse
    {
        [JsonProperty("bsvalias")]
        public string BsvAlias { get; set; }

        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("pubkey")]
        public string PubKey { get; set; }
    }
}

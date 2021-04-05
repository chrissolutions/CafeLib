using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.Paymail.Models
{
    internal class VerifyPubKeyResponse
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("pubkey")]
        public string PubKey { get; set; }

        [JsonProperty("match")]
        public bool Match { get; set; }
    }
}

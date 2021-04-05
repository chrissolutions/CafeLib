using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.Paymail.Models
{
    internal class GetOutputScriptRequest
    {
        [JsonProperty("senderHandle")]
        public string SenderHandle { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("dt")]
        public string Dt { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        [JsonProperty("senderName")]
        public string SenderName { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}

using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.Paymail.Models
{
    internal class GetOutputScriptResponse
    {
        [JsonProperty("output")]
        public string Output { get; set; }
    }
}

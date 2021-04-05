using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CafeLib.Bitcoin.Api.Paymail
{
    public class CapabilitiesResponse
    {
        [JsonProperty("bsvalias")]
        public string BsvAlias { get; set; }

        [JsonProperty("capabilities")]
        public Dictionary<string, JValue> Capabilities { get; set; } = new Dictionary<string, JValue>();
    }
}

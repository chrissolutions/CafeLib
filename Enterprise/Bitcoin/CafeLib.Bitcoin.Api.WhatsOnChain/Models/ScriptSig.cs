using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain.Models
{
    public class ScriptSig
    {
        [JsonProperty("asm")]
        public string Asm { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }
    }
}
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain.Models
{
    public class Input
    {
        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("vout")]
        public int VectorOutput { get; set; }

        [JsonProperty("scriptSig")]
        public ScriptSig ScriptSig { get; set; }

        [JsonProperty("sequence")]
        public uint Sequence { get; set; }

        [JsonProperty("coinbase")]
        public string CoinBase { get; set; }
    }
}
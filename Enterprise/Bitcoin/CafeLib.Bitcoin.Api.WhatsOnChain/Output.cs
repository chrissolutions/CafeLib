using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain
{
    public class Vout
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("n")]
        public int n { get; set; }

        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("vout")]
        public int Vout { get; set; }

        [JsonProperty("scriptSig")]
        public ScriptSig ScriptSig { get; set; }

        [JsonProperty("sequence")]
        public uint Sequence { get; set; }

        [JsonProperty("coinbase")]
        public string CoinBase { get; set; }
    }
}
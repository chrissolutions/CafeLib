using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain
{
    public class Transaction
    {
        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("locktime")]
        public uint LockTime { get; set; }

        [JsonProperty("vin")]
        public Vin[] Vin { get; set; }

        [JsonProperty("vout")]
        public Vout[] Vout { get; set; }

        [JsonProperty("blockhash")]
        public string BlockHash { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("blocktime")]
        public long BlockTime { get; set; }
    }
}
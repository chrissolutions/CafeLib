﻿using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain.Models
{
    public class Transaction
    {
        [JsonProperty("hex")]
        public string Hex { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("locktime")]
        public uint LockTime { get; set; }

        [JsonProperty("vin")]
        public Input[] VectorInput { get; set; }

        [JsonProperty("vout")]
        public VectorOutput VectorOutput { get; set; }

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
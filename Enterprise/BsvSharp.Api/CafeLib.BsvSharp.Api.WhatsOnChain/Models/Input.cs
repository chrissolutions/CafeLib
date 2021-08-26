﻿using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain.Models
{
    public class Input
    {
        [JsonProperty("coinbase")]
        public string CoinBase { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("vout")]
        public int VectorOutput { get; set; }

        [JsonProperty("scriptSig")]
        public ScriptSig ScriptSig { get; set; }

        [JsonProperty("sequence")]
        public uint Sequence { get; set; }
    }
}
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain.Models
{
    public class ByAddressUnspent
    {
        [JsonProperty("height")]
        public int Height;

        [JsonProperty("tx_pos")]
        public int TransactionPosition;

        [JsonProperty("tx_hash")]
        public string TransactionHash;

        [JsonProperty("value")]
        public long Value;
    }
}
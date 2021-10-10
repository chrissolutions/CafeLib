using Newtonsoft.Json;

namespace CafeLib.BsvSharp.Mapi.Models
{
    public class Fee
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("feeType")]
        public string FeeType { get; set; }

        [JsonProperty("miningFee")]
        public FeeInfo MiningFee { get; set; }

        [JsonProperty("relayFee")]
        public FeeInfo RelayFee { get; set; }
    }
}

using Newtonsoft.Json;

namespace CafeLib.BsvSharp.Mapi.Models
{
    public class Quotes
    {
        [JsonProperty("quotes")]
        public ProviderQuote[] ProviderQuotes { get; set; }
    }
}

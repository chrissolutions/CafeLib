using Newtonsoft.Json;

namespace AtmAgentChequeUpload.Models
{
    public class ContentElement
    {
        public ContentElement(
            string contentType = null, 
            string name = null, 
            long? contentSize = null,
            string content = null)
        {

        }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "contentSize")]
        public long? ContentSize { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}

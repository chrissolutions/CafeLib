using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AtmAgentChequeUpload.Models
{
    public class DocumentUploadDto
    {
        public DocumentUploadDto()
        {

        }

        public DocumentUploadDto(
            string documentProcessTypeCode, 
            string documentTypeCode, 
            string channelTypeCode,
            IDictionary<string, object> documentMetadata, 
            int? staffNumber = null,
            IList<ContentElement> contentElements = null)
        {

        }

        [JsonProperty(PropertyName = "documentProcessTypeCode")]
        public string DocumentProcessTypeCode { get; set; }
        [JsonProperty(PropertyName = "documentTypeCode")]
        public string DocumentTypeCode { get; set; }
        [JsonProperty(PropertyName = "channelTypeCode")]
        public string ChannelTypeCode { get; set; }
        [JsonProperty(PropertyName = "staffNumber")]
        public int? StaffNumber { get; set; }
        [JsonProperty(PropertyName = "contentElements")]
        public IList<ContentElement> ContentElements { get; set; }
        [JsonProperty(PropertyName = "documentMetadata")]
        public IDictionary<string, object> DocumentMetadata { get; set; }

        public virtual void Validate()
        {

        }
    }
}

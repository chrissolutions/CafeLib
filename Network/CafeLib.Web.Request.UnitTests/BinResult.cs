using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request.UnitTests
{
    public class BinResult
    {
        public JToken Args { get; set; }
        public string Data { get; set; }
        public JToken Files { get; set; }
        public JToken Form { get; set; }
        public JToken Headers { get; set; }
        public JToken Json { get; set; }
        public string Method { get; set; }
        public string Origin { get; set; }
        public string Url { get; set; }
    }
}

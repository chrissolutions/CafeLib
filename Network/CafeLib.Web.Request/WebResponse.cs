using System;
using System.Linq;
using System.Net.Http;

namespace CafeLib.Web.Request
{
    public class WebResponse
    {
        public int StatusCode { get; }

        public string ReasonPhrase { get; }

        public WebHeaders ContentHeaders { get; }

        public string RequestMethod { get; }

        public Uri RequestUri { get; }

        public WebHeaders RequestHeaders { get; }

        public WebHeaders ResponseHeaders { get; }

        internal string ContentType { get; }

        internal WebResponse(HttpResponseMessage response)
        {
            StatusCode = (int)response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            RequestMethod = response.RequestMessage.Method.ToString();
            RequestUri = response.RequestMessage.RequestUri;
            ContentHeaders = new WebHeaders(response.Content.Headers);
            RequestHeaders = new WebHeaders(response.RequestMessage.Headers);
            ResponseHeaders = new WebHeaders(response.Headers);
            ContentType = GetContentType();
        }

        internal string GetContentType()
        {
            if (ContentHeaders.TryGetValue("Content-Type", out var contentTypes))
            {
                var contentType = contentTypes.First();
                if (contentType.Contains("json"))
                    return WebContentType.Json;

                if (contentType.Contains("xml"))
                    return WebContentType.Xml;

                if (contentType.Contains("html"))
                    return WebContentType.Html;

                if (contentType.Contains("octet"))
                    return WebContentType.Octet;
            }

            return WebContentType.Text;
        }

        internal WebResponse EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new WebRequestException(this);
            }

            return this;
        }
    }
}

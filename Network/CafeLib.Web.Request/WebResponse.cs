using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CafeLib.Core.Support;

namespace CafeLib.Web.Request
{
    public class WebResponse
    {
        private readonly HttpResponseMessage _response;

        public int StatusCode { get; }

        public string ReasonPhrase { get; }

        public WebHeaders ContentHeaders { get; }

        public string RequestMethod { get; }

        public Uri RequestUri { get; }

        public WebHeaders RequestHeaders { get; }

        public WebHeaders ResponseHeaders { get; }

        internal WebResponse(HttpResponseMessage response)
        {
            _response = response;
            StatusCode = (int)response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            RequestMethod = response.RequestMessage.Method.ToString();
            RequestUri = response.RequestMessage.RequestUri;
            ContentHeaders = new WebHeaders(response.Content.Headers);
            RequestHeaders = new WebHeaders(response.RequestMessage.Headers);
            ResponseHeaders = new WebHeaders(response.Headers);
        }

        internal Task<Stream> GetContent()
        {
            return _response.Content.ReadAsStreamAsync();
        }

        internal WebResponse EnsureSuccessStatusCode()
        {
            if (!_response.IsSuccessStatusCode)
            {
                throw new WebRequestException(this);
            }

            return this;
        }
    }
}

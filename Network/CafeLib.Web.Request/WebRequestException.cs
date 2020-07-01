using System;

namespace CafeLib.Web.Request
{
    public class WebRequestException : Exception
    {
        public WebResponse Response { get; }

        internal WebRequestException(WebResponse response)
            : base($"Unsuccessful status code {response.StatusCode}, reason {response.ReasonPhrase}.")
        {
            Response = response;
        }
    }
}

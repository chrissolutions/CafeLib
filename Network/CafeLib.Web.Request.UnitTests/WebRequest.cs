using System;
using System.Threading.Tasks;

namespace CafeLib.Web.Request.UnitTests
{
    public class WebRequest<TResponse> : WebRequestBase, IWebRequest<TResponse>
    {
        public WebRequest(string endpoint, WebHeaders headers = null)
            : this(new Uri(endpoint), headers)
        {
        }

        public WebRequest(Uri endpoint, WebHeaders headers = null) 
            : base(endpoint, headers)
        {
            SetHeaders();
        }

        public Task<TResponse> GetAsync(WebHeaders headers = null, object parameters = null)
        {
            return GetAsync<TResponse>(headers, parameters);
        }

        public Task<TResponse> PostAsync<TIn>(TIn request, WebHeaders headers = null, object parameters = null)
        {
            return PostAsync<TIn, TResponse>(request, headers, parameters);
        }

        public Task<TResponse> PutAsync<TIn>(TIn request, WebHeaders headers = null, object parameters = null)
        {
            return PutAsync<TIn, TResponse>(request, headers, parameters);
        }

        public Task<TResponse> DeleteAsync<TIn>(TIn request, WebHeaders headers = null, object parameters = null)
        {
            return DeleteAsync<TIn, TResponse>(request, headers, parameters);
        }

        private void SetHeaders()
        {
            Headers.Add("Authorization", "Basic Og==");
            Headers.Add("Content-Type", "application/json");
            Headers.Add("Accept", "*/*");
            Headers.Add("Connection", "keep-alive");
            Headers.Add("Cache-Control", "no-cache");
            Headers.Add("User-Agent", "PostmanRuntime/7.26.1");
        }
    }
}

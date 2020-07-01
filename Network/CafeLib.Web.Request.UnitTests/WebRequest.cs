using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CafeLib.Web.Request.UnitTests
{
    public class WebRequest<TResponse> : WebRequestBase, IWebRequest<TResponse>
    {
        public WebRequest(string endpoint, WebRequestHeaders headers = null)
            : base(endpoint, headers)
        {
            SetHeaders();
        }

        public WebRequest(string serverUrl, string path, WebRequestHeaders headers = null) 
            : base(serverUrl, path, headers)
        {
            SetHeaders();
        }

        public WebRequest(Uri endpoint, WebRequestHeaders headers = null) 
            : base(endpoint, headers)
        {
            SetHeaders();
        }

        public Task<TResponse> GetAsync(WebRequestHeaders headers = null, object parameters = null)
        {
            return GetAsync<TResponse>(headers, parameters);
        }

        public Task<TResponse> PostAsync<TIn>(TIn request, WebRequestHeaders headers = null, object parameters = null)
        {
            return PostAsync<TIn, TResponse>(request, headers, parameters);
        }

        public Task<TResponse> PutAsync<TIn>(TIn request, WebRequestHeaders headers = null, object parameters = null)
        {
            return PutAsync<TIn, TResponse>(request, headers, parameters);
        }

        public Task<TResponse> DeleteAsync<TIn>(TIn request, WebRequestHeaders headers = null, object parameters = null)
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

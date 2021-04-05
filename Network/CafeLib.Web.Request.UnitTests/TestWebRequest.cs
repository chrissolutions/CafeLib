using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request.UnitTests
{
    public class TestWebRequest<TResponse> : WebRequestBase
    {
        public Uri Endpoint { get; protected set; }

        /// <summary>
        /// ApiRequest constructor
        /// </summary>
        /// <param name="endpoint">service endpoint</param>
        public TestWebRequest(string endpoint)
            : this(new Uri(endpoint))
        {
        }

        /// <summary>
        /// WebRequestBase constructor
        /// </summary>
        /// <param name="endpoint">service endpoint</param>
        public TestWebRequest(Uri endpoint)
        {
            Endpoint = endpoint;
            SetHeaders();
        }

        public Task<TResponse> GetAsync(WebHeaders headers = null, object parameters = null)
        {
            return GetAsync<TResponse>(Endpoint, headers, parameters);
        }

        public Task<TResponse> PostAsync(JToken body, WebHeaders headers = null, object parameters = null)
        {
            return PostAsync<JToken, TResponse>(Endpoint, body, headers, parameters);
        }

        public Task<TResponse> PutAsync(JToken body, WebHeaders headers = null, object parameters = null)
        {
            return PutAsync<JToken, TResponse>(Endpoint, body, headers, parameters);
        }

        public Task<bool> DeleteAsync(JToken body, WebHeaders headers = null, object parameters = null)
        {
            return DeleteAsync(Endpoint, body, headers, parameters);
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

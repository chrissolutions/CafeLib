using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request
{
    public class JsonRequest<TResponse> : WebRequestBase, IWebRequest<TResponse, JToken>
    {
        public JsonRequest(string endpoint, WebHeaders headers = null)
            : this(new Uri(endpoint), headers)
        {
        }

        public JsonRequest(Uri endpoint, WebHeaders headers = null) 
            : base(endpoint, headers)
        {
        }

        public Task<TResponse> GetAsync(WebHeaders headers = null, object parameters = null)
        {
            return GetAsync<TResponse>(headers, parameters);
        }

        public Task<TResponse> PostAsync(JToken body, WebHeaders headers = null, object parameters = null)
        {
            return PostAsync<JToken, TResponse>(body, headers, parameters);
        }

        public Task<TResponse> PutAsync(JToken body, WebHeaders headers = null, object parameters = null)
        {
            return PutAsync<JToken, TResponse>(body, headers, parameters);
        }

        public Task<bool> DeleteAsync(JToken body, WebHeaders headers = null, object parameters = null)
        {
            return DeleteAsync<JToken>(body, headers, parameters);
        }
    }
}

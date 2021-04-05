using System;
using System.Threading.Tasks;

namespace CafeLib.Web.Request
{
    public class ApiRequest<TResponse, TBody> : WebRequestBase, IApiRequest<TResponse, TBody>
    {
        public ApiRequest()
        {
            Headers = new WebHeaders();
        }

        public ApiRequest(WebHeaders headers)
        {
            Headers = headers ?? new WebHeaders();
        }

        public Task<TResponse> GetAsync(string endpoint, WebHeaders headers = null, object parameters = null)
        {
            return GetAsync(new Uri(endpoint), headers, parameters);
        }

        public Task<TResponse> GetAsync(Uri endpoint, WebHeaders headers = null, object parameters = null)
        {
            return GetAsync<TResponse>(endpoint, headers, parameters);
        }

        public Task<TResponse> PostAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null)
        {
            return PostAsync(new Uri(endpoint), body, headers, parameters);
        }

        public Task<TResponse> PostAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null)
        {
            return PostAsync<TBody, TResponse>(endpoint, body, headers, parameters);
        }

        public Task<TResponse> PutAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null)
        {
            return PutAsync(new Uri(endpoint), body, headers, parameters);
        }

        public Task<TResponse> PutAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null)
        {
            return PutAsync<TBody, TResponse>(body, headers, parameters);
        }

        public Task<bool> DeleteAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null)
        {
            return DeleteAsync(new Uri(endpoint), body, headers, parameters);
        }

        public Task<bool> DeleteAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null)
        {
            return DeleteAsync<TBody>(endpoint, body, headers, parameters);
        }
    }
}

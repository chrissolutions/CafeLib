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

        public Task<TResponse> GetAsync(string endpoint, WebHeaders headers = null, object parameters = null) =>
            GetAsync(new Uri(endpoint), headers, parameters);

        public Task<TResponse> GetAsync(Uri endpoint, WebHeaders headers = null, object parameters = null) =>
            GetAsync<TResponse>(endpoint, headers, parameters);

        public Task<TResponse> PostAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null) =>
            PostAsync(new Uri(endpoint), body, headers, parameters);

        public Task<TResponse> PostAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null) =>
            PostAsync<TBody, TResponse>(endpoint, body, headers, parameters);

        public Task<TResponse> PutAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null) =>
            PutAsync(new Uri(endpoint), body, headers, parameters);

        public Task<TResponse> PutAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null) =>
            PutAsync<TBody, TResponse>(endpoint, body, headers, parameters);

        public Task<bool> DeleteAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null) =>
            DeleteAsync(new Uri(endpoint), body, headers, parameters);

        public Task<bool> DeleteAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null) =>
            DeleteAsync<TBody>(endpoint, body, headers, parameters);
    }
}

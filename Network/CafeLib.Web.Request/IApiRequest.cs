using System;
using System.Threading.Tasks;

namespace CafeLib.Web.Request
{
    public interface IApiRequest<TResponse, in TBody>
    {
        Task<TResponse> GetAsync(string endpoint, WebHeaders headers = null, object parameters = null);
        Task<TResponse> GetAsync(Uri endpoint, WebHeaders headers = null, object parameters = null);

        Task<TResponse> PostAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null);
        Task<TResponse> PostAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null);

        Task<TResponse> PutAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null);
        Task<TResponse> PutAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null);

        Task<bool> DeleteAsync(string endpoint, TBody body, WebHeaders headers = null, object parameters = null);
        Task<bool> DeleteAsync(Uri endpoint, TBody body, WebHeaders headers = null, object parameters = null);
    }
}

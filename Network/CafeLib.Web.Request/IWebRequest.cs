using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request
{
    public interface IWebRequest<TResponse, in TBody>
    {
        Task<TResponse> GetAsync(WebHeaders headers = null, object parameters = null);

        Task<TResponse> PostAsync(TBody body, WebHeaders headers = null, object parameters = null);

        Task<TResponse> PutAsync(TBody body, WebHeaders headers = null, object parameters = null);

        Task<bool> DeleteAsync(TBody body, WebHeaders headers = null, object parameters = null);
    }
}

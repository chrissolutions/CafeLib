using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request
{
    public interface IWebRequest<TResponse>
    {
        Task<TResponse> GetAsync(WebHeaders headers = null, object parameters = null);

        Task<TResponse> PostAsync(JToken body, WebHeaders headers = null, object parameters = null);

        Task<TResponse> PutAsync(JToken body, WebHeaders headers = null, object parameters = null);

        Task<bool> DeleteAsync(JToken body, WebHeaders headers = null, object parameters = null);
    }
}

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request.UnitTests
{
    public interface IWebRequest<T>
    {
        Task<T> GetAsync(WebHeaders headers = null, object parameters = null);

        Task<T> PostAsync(JToken body, WebHeaders headers = null, object parameters = null);

        Task<T> PutAsync(JToken body, WebHeaders headers = null, object parameters = null);

        Task<T> DeleteAsync(JToken body, WebHeaders headers = null, object parameters = null);
    }
}

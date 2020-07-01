using System.Threading.Tasks;

namespace CafeLib.Web.Request.UnitTests
{
    public interface IWebRequest<TOut>
    {
        Task<TOut> GetAsync(WebRequestHeaders headers = null, object parameters = null);

        Task<TOut> PostAsync<TIn>(TIn request, WebRequestHeaders headers = null, object parameters = null);

        Task<TOut> PutAsync<TIn>(TIn request, WebRequestHeaders headers = null, object parameters = null);

        Task<TOut> DeleteAsync<TIn>(TIn request, WebRequestHeaders headers = null, object parameters = null);
    }
}

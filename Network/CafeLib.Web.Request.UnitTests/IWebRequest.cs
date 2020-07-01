using System.Threading.Tasks;

namespace CafeLib.Web.Request.UnitTests
{
    public interface IWebRequest<TOut>
    {
        Task<TOut> GetAsync(WebHeaders headers = null, object parameters = null);

        Task<TOut> PostAsync<TIn>(TIn request, WebHeaders headers = null, object parameters = null);

        Task<TOut> PutAsync<TIn>(TIn request, WebHeaders headers = null, object parameters = null);

        Task<TOut> DeleteAsync<TIn>(TIn request, WebHeaders headers = null, object parameters = null);
    }
}

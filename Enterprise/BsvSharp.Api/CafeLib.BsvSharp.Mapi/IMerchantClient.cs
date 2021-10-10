using System.Threading.Tasks;
using CafeLib.BsvSharp.Mapi.Models;
using CafeLib.Web.Request;

namespace CafeLib.BsvSharp.Mapi 
{
    public interface IMerchantClient
    {
        Task<ApiResponse<ProviderQuote>> GetFeeQuote();

        Task<ApiResponse<TransactionStatus>> GetTransactionStatus(string txHash);

        Task<ApiResponse<TransactionResponse>> SubmitTransaction(string txHash);
    }
}

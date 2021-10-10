using System;
using System.Threading.Tasks;
using CafeLib.BsvSharp.Mapi.Models;
using CafeLib.BsvSharp.Network;
using CafeLib.Core.Extensions;
using CafeLib.Web.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CafeLib.BsvSharp.Mapi
{
    public abstract class MerchantClient : BasicApiRequest, IMerchantClient
    {
        public string Network { get; }
        public string Url { get; }
        public string Name { get; }

        protected MerchantClient(string clientName, string merchantUrl, NetworkType networkType = NetworkType.Main)
        {
            Name = clientName;
            Url = merchantUrl;
            Network = networkType.GetDescriptor();

            Headers.Add("Content-Type", WebContentType.Json);
            Headers.Add("User-Agent", typeof(MerchantClient).Namespace);
        }

        #region Mapi

        public virtual async Task<ApiResponse<ProviderQuote>> GetFeeQuote()
        {
            try
            {
                var url = $"{Url}/mapi/feeQuote";
                var json = await GetAsync(url);
                var response = JsonConvert.DeserializeObject<ProviderQuote>(json);
                if (response == null) throw new MerchantClientException<ProviderQuote>("null response");
                response.ProviderName = Name;
                response.Quote = JsonConvert.DeserializeObject<Quote>(response.Payload);
                if (response.Quote == null) throw new MerchantClientException<ProviderQuote>(response, "missing payload");
                response.ProviderId = response.Quote.MinerId;
                return new ApiResponse<ProviderQuote>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProviderQuote>(ex);
            }
        }

        public virtual async Task<ApiResponse<TransactionStatus>> GetTransactionStatus(string txHash)
        {
            try
            {
                var url = $"{Url}/mapi/tx/{txHash}";
                var json = await GetAsync(url);
                var response = JsonConvert.DeserializeObject<TransactionStatus>(json);
                if (response == null) throw new MerchantClientException<TransactionStatus>("null response");
                var status = JsonConvert.DeserializeObject<TransactionStatus>(response.Payload);
                if (status == null) throw new MerchantClientException<TransactionStatus>(response, "missing payload");
                status.Payload = response.Payload;
                status.PublicKey = response.PublicKey;
                status.Signature = response.Signature;
                status.Encoding = response.Encoding;
                status.MimeType = response.MimeType;
                return new ApiResponse<TransactionStatus>(status);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TransactionStatus>(ex);
            }
        }

        public virtual async Task<ApiResponse<TransactionResponse>> SubmitTransaction(string txHash)
        {
            try
            {
                var url = $"{Url}/mapi/tx";
                var jsonBody = JToken.FromObject(new { rawTx = txHash });
                var json = await PostAsync(url, jsonBody);
                var response = JsonConvert.DeserializeObject<TransactionResponse>(json);
                return new ApiResponse<TransactionResponse>(response);
            }   
            catch (Exception ex)
            {
                return new ApiResponse<TransactionResponse>(ex);
            }
        }

        #endregion
    }
}

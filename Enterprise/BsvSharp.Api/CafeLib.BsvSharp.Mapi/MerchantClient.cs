using System;
using System.Threading.Tasks;
using CafeLib.BsvSharp.Mapi.Models;
using CafeLib.BsvSharp.Mapi.Responses;
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

        public virtual async Task<ApiResponse<FeeQuoteResponse>> GetFeeQuote()
        {
            try
            {
                var url = $"{Url}/mapi/feeQuote";
                var json = await GetAsync(url);
                var response = JsonConvert.DeserializeObject<FeeQuoteResponse>(json);
                if (response == null) throw new MerchantClientException<FeeQuoteResponse>("null response");

                response.ProviderName = Name;
                response.Cargo = JsonConvert.DeserializeObject<Quote>(response.Payload);
                if (response.Cargo == null) throw new MerchantClientException<FeeQuoteResponse>(response, "missing payload");
                response.ProviderId = response.Cargo.MinerId;
                return new ApiResponse<FeeQuoteResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FeeQuoteResponse>(ex);
            }
        }

        public virtual async Task<ApiResponse<TransactionStatusResponse>> GetTransactionStatus(string txHash)
        {
            try
            {
                var url = $"{Url}/mapi/tx/{txHash}";
                var json = await GetAsync(url);
                var response = JsonConvert.DeserializeObject<TransactionStatusResponse>(json);
                if (response == null) throw new MerchantClientException<TransactionStatus>("null response");

                response.ProviderName = Name;
                response.Cargo = JsonConvert.DeserializeObject<TransactionStatus>(response.Payload);
                if (response.Cargo == null) throw new MerchantClientException<TransactionStatusResponse>(response, "missing payload");
                response.ProviderId = response.Cargo.MinerId;
                return new ApiResponse<TransactionStatusResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TransactionStatusResponse>(ex);
            }
        }

        public virtual async Task<ApiResponse<TransactionSubmitResponse>> SubmitTransaction(string txRaw)
        {
            try
            {
                var url = $"{Url}/mapi/tx";
                var jsonBody = JToken.FromObject(new { rawtx = txRaw });
                var json = await PostAsync(url, jsonBody);
                var response = JsonConvert.DeserializeObject<TransactionSubmitResponse>(json);
                if (response == null) throw new MerchantClientException<TransactionStatus>("null response");

                response.ProviderName = Name;
                response.Cargo = JsonConvert.DeserializeObject<TransactionSubmit>(response.Payload);
                if (response.Cargo == null) throw new MerchantClientException<TransactionSubmitResponse>(response, "missing payload");
                return new ApiResponse<TransactionSubmitResponse>(response);
            }   
            catch (Exception ex)
            {
                return new ApiResponse<TransactionSubmitResponse>(ex);
            }
        }

        #endregion
    }
}

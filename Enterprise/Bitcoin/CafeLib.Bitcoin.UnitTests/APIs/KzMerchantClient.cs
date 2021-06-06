#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Encoding;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Units;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.UnitTests.APIs
{
    /// <summary>
    /// </summary>
    public class KzMerchantClient
    {
        public static string UserAgent = "KzMerchantClient";

        //public static string EndpointPreferred = "https://www.ddpurse.com/openapi,561b756d12572020ea9a104c3441b71790acbbce95a6ddbf7e0630971af9424b";
        //public static string EndpointPreferred = "https://merchantapi.taal.com";
        public static string EndpointPreferred = "https://merchantapi.matterpool.io";

        public static string EndpointTaal = "https://merchantapi.taal.com";
        public static string EndpointMempool = "https://www.ddpurse.com/openapi,561b756d12572020ea9a104c3441b71790acbbce95a6ddbf7e0630971af9424b";
        public static string EndpointMatterPool = "https://merchantapi.matterpool.io";

        /// <summary>
        /// Returns client for current preferred endpoint.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static KzMerchantClient GetClient(string baseUrl = null) => new KzMerchantClient(baseUrl ?? EndpointPreferred);

        public static KzMerchantClient GetClientForTaal() => new KzMerchantClient(EndpointTaal);
        public static KzMerchantClient GetClientForMempool() => new KzMerchantClient(EndpointMempool);
        public static KzMerchantClient GetClientForMatterPool() => new KzMerchantClient(EndpointMatterPool);

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private PublicKey _pubKey;

        public KzMerchantClient(string baseUrl, PublicKey pubKey = null)
        {
            var parts = baseUrl.Split(',');
            var token = string.Empty;
            if (parts.Length == 2)
            {
                token = parts[1];
                baseUrl = parts[0];
            }
            _pubKey = pubKey;
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            if (token != string.Empty)
                _httpClient.DefaultRequestHeaders.Add("token", token);
        }

        private FeeQuote _lastFeeQuote;

        /// <summary>
        /// https://github.com/bitcoin-sv-specs/brfc-misc/tree/master/feespec
        /// </summary>
        /// <returns></returns>
        public async Task<FeeQuote> GetFeeQuote()
        {
            var feeQuote = _lastFeeQuote;

            if (feeQuote?.expiryTime > DateTime.UtcNow)
                return feeQuote;

            var url = $"{_baseUrl}/mapi/feeQuote";

            var json = await _httpClient.GetStringAsync(url);

            var e = JsonConvert.DeserializeObject<Envelope>(json);

            var verified = e.VerifySignature(ref _pubKey);

            feeQuote = JsonConvert.DeserializeObject<FeeQuote>(e.Payload);

            if (!verified && feeQuote.minerId != null)
                throw new InvalidOperationException("Miner did not verify.");

            _lastFeeQuote = feeQuote;

            return feeQuote;
        }

        public static Amount ComputeFee((long tx, long data) lengths, (FeeQuoteRate standard, FeeQuoteRate data) rates)
        {
            return rates.standard.ComputeFee(lengths.tx - lengths.data) + rates.data.ComputeFee(lengths.data);
        }

        public class FeeQuoteRate
        {
            [JsonProperty("satoshis")]
            public int Satoshis { get; set; }

            [JsonProperty("bytes")]
            public int Bytes { get; set; }

            public Amount ComputeFee(long length)
            {
                var fee = (length * Satoshis) / Bytes;
                if (length > 0 && fee == 0 && Satoshis != 0) fee = 1;
                return new Amount(fee);
            }
        }

        public class FeeQuoteType
        {
            [JsonProperty("feeType")]
            public string FeeType { get; set; }

            [JsonProperty("miningFee")]
            public FeeQuoteRate MiningFee { get; set; }

            [JsonProperty("RelayFee")]
            public FeeQuoteRate RelayFee { get; set; }
        }

        public class FeeQuote
        {
            #region Example of feeQuote Envelope.payload JSON:
#if false

{
    "apiVersion":"0.1.0",
    "timestamp":"2020-04-18T17:23:29.123Z",
    "expiryTime":"2020-04-18T17:33:29.123Z",
    "minerId":"03e92d3e5c3f7bd945dfbf48e7a99393b1bfb3f11f380ae30d286e7ff2aec5a270",
    "currentHighestBlockHash":"0000000000000000002d68c182c1b4aca3a6e3f1b5f9bf3ff25eb14d6294fd6b",
    "currentHighestBlockHeight":631170,
    "minerReputation":null,
    "fees":
    [
        {
            "feeType":"standard",
            "miningFee":{"satoshis":5,"bytes":10},
            "relayFee":{"satoshis":25,"bytes":100}
        },
        {
            "feeType":"data",
            "miningFee":{"satoshis":5,"bytes":10},
            "relayFee":{"satoshis":25,"bytes":100}
        }
    ]
}
#endif
            #endregion
            public string apiVersion;
            public DateTime timestamp;
            public DateTime expiryTime;
            public string minerId;
            public string currentHighestBlockHash;
            public int currentHighestBlockHeight;
            public string minerReputation;
            public FeeQuoteType[] fees;

            public (FeeQuoteRate standard, FeeQuoteRate data) MiningRates => (fees?.SingleOrDefault(f => f.FeeType == "standard")?.MiningFee, fees?.SingleOrDefault(f => f.FeeType == "data")?.MiningFee);

            public (FeeQuoteRate standard, FeeQuoteRate data) RelayRates => (fees?.SingleOrDefault(f => f.FeeType == "standard")?.RelayFee, fees?.SingleOrDefault(f => f.FeeType == "data")?.RelayFee);

            /// <summary>
            /// Compute Mining Fee.
            /// </summary>
            /// <param name="txLength">Serialized transaction size in bytes.</param>
            /// <param name="dataLength">Sum of output script bytes that begin with OP_FALSE OP_RETURN.</param>
            /// <returns></returns>
            public Amount ComputeMiningFee(long txLength, long dataLength) => ComputeFee((txLength, dataLength), MiningRates);

            public Amount ComputeRelayFee(long txLength, long dataLength) => ComputeFee((txLength, dataLength), RelayRates);

        }

        public async Task<TransactionStatus> GetTransactionStatus(string hashTx, KzServiceRequestLog srl = null)
        {
            var url = $"{_baseUrl}/mapi/tx/{hashTx}";

            if (srl != null)
            {
                srl.When = DateTime.UtcNow;
                srl.ServiceEndpoint = url;
            }

            var json = await _httpClient.GetStringAsync(url);

            if (srl != null)
                srl.ServiceResponse = json;

            var e = JsonConvert.DeserializeObject<Envelope>(json);

            var status = (TransactionStatus)null;

            var verified = e.VerifySignature(ref _pubKey);

            status = JsonConvert.DeserializeObject<TransactionStatus>(e.Payload);

            if (srl != null)
            {
                srl.Verified = verified;
                srl.Success = status?.returnResult == "success";
            }

            return status;
        }

        public class TransactionStatus
        {
            #region Example of Transaction Status JSON:
#if false
{
    "apiVersion":"0.1.0",
    "timestamp":"2020-04-21T12:32:03.211Z",
    "returnResult":"success",
    "resultDescription":"",
    "blockHash":"0000000000000000011e0221844b65bfbbc2599bbd7f71ca0f914a53d90fa8b6",
    "blockHeight":631498,
    "confirmations":92,
    "minerId":"03c51d59a737a0ebc064344bf206b7140bf51a9ef8d6cb75dc2d726853d7c76758",
    "txSecondMempoolExpiry":0
}
{
    "apiVersion":"0.1.0",
    "timestamp":"2020-04-21T14:20:24.230Z",
    "returnResult":"failure",
    "resultDescription":"ERROR: No such mempool or blockchain transaction. Use gettransaction for wallet transactions.",
    "blockHash":null,
    "blockHeight":null,
    "confirmations":0,
    "minerId":"03c51d59a737a0ebc064344bf206b7140bf51a9ef8d6cb75dc2d726853d7c76758",
    "txSecondMempoolExpiry":0
}
#endif
            #endregion
            public string apiVersion;
            public DateTime timestamp;
            public string returnResult;
            public string resultDescription;
            public string blockHash;
            public int? blockHeight;
            public int confirmations;
            public string minerId;
            public int txSecondMempoolExpiry;
        }

        async public Task<PostTransactionResponse> PostTransaction(byte[] txBytes, KzServiceRequestLog srl)
        {

            var ptr = (PostTransactionResponse)null;

            var url = $"{_baseUrl}/mapi/tx";

            srl.When = DateTime.UtcNow;
            srl.ServiceEndpoint = url;

            var txHex = Encoders.Hex.Encode(txBytes);
            //var hashTx = KzHashes.HASH256(txBytes);

            var jsonContent = JsonConvert.SerializeObject(new { rawtx = txHex });
            //var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // Both MatterPool and Taal don't handle the encoding header (charset=utf-8), returning http request status of 400.
            // So for now, don't include an encoding header.
            var httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var rm = await _httpClient.PostAsync(url, httpContent);

            srl.ServiceResponse = rm?.StatusCode.ToString();

            if (rm?.StatusCode == HttpStatusCode.OK)
            {
                var response = await rm.Content.ReadAsStringAsync();

                srl.ServiceResponse = response;

                var e = JsonConvert.DeserializeObject<Envelope>(response);

                srl.Verified = e.VerifySignature(ref _pubKey);

                ptr = JsonConvert.DeserializeObject<PostTransactionResponse>(e.Payload);

                srl.Success = ptr?.returnResult == "success";
            }

            return ptr;
        }

        public class PostTransactionResponse
        {
            public string apiVersion;
            public DateTime timestamp;
            public string txid;
            public string returnResult;
            public string resultDescription;
            public string minerId;
            public string currentHighestBlockHash;
            public int currentHighestBlockHeight;
            public int txSecondMempoolExpiry;

            public override string ToString()
            {
                return System.Text.Json.JsonSerializer.Serialize(this);
            }
        }


        public class Envelope
        {
            #region Example of Envelope JSON:
#if false
 {
     "payload":"{\"apiVersion\":\"0.1.0\",\"timestamp\":\"2020-04-18T16:13:20.092Z\",\"expiryTime\":\"2020-04-18T16:23:20.092Z\",\"minerId\":\"03e92d3e5c3f7bd945dfbf48e7a99393b1bfb3f11f380ae30d286e7ff2aec5a270\",\"currentHighestBlockHash\":\"00000000000000000114fa4b61a7b6dd62bc911cd6b88f74202d75dbbc4c1fc5\",\"currentHighestBlockHeight\":631159,\"minerReputation\":null,\"fees\":[{\"feeType\":\"standard\",\"miningFee\":{\"satoshis\":5,\"bytes\":10},\"relayFee\":{\"satoshis\":25,\"bytes\":100}},{\"feeType\":\"data\",\"miningFee\":{\"satoshis\":5,\"bytes\":10},\"relayFee\":{\"satoshis\":25,\"bytes\":100}}]}",
     "signature":"3044022017c5cf508ce99d82664ad9508105589859fa3ae611a80afeb42f6b7bbb631364022077dfd53d02d73b1aa1dcf6ab4d04691b11a9625b8c690aabb782b0e73bc8ef7a",
     "publicKey":"03e92d3e5c3f7bd945dfbf48e7a99393b1bfb3f11f380ae30d286e7ff2aec5a270",
     "encoding":"UTF-8",
     "mimetype":"application/json"
 }
#endif
            #endregion
            [JsonProperty("payload")]
            public string Payload { get; set; }

            [JsonProperty("signature")]
            public string Signature { get; set; }

            [JsonProperty("publicKey")]
            public string PublicKey { get; set; }

            [JsonProperty("encoding")]
            public string Encoding { get; set; }

            [JsonProperty("mimetype")]
            public string MimeType { get; set; }

            public bool VerifySignature(ref PublicKey pubKey)
            {

                var verifyHash = Hashes.Sha256(Payload.Utf8ToBytes());
                var verifySignature = Signature?.HexToBytes();
                pubKey ??= new PublicKey(PublicKey?.HexToBytes());
                var verified = pubKey?.Verify(verifyHash, verifySignature) ?? false;

                return verified;
            }
        }
    }
}


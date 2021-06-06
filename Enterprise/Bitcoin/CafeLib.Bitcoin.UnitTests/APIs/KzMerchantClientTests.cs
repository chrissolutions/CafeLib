#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Numerics;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.APIs {
    public class KzMerchantClientTests
    {

        [Theory]
        [InlineData(null, "0.1.0")]
        [InlineData("https://merchantapi.matterpool.io", "0.1.0")]
//        [InlineData("https://www.ddpurse.com/openapi,561b756d12572020ea9a104c3441b71790acbbce95a6ddbf7e0630971af9424b")]
        [InlineData("https://merchantapi.taal.com", "1.1.0")]
        public async Task MapiGetFeeQuoteTest(string baseUrl, string version) {
            KzMerchantClient.UserAgent = "KzMerchantClientTest";
            var mapi = KzMerchantClient.GetClient(baseUrl);
            var feeQuote = await mapi.GetFeeQuote();
            Assert.NotNull(feeQuote);
            Assert.True(feeQuote.ExpiryTime > DateTime.UtcNow);
            Assert.True(Math.Abs((feeQuote.Timestamp - DateTime.UtcNow).TotalMinutes) < 1);
            Assert.Equal(version, feeQuote.ApiVersion);
            Assert.True(feeQuote.CurrentHighestBlockHeight > 630000);
            Assert.True(new UInt256(feeQuote.CurrentHighestBlockHash).ToBigInteger() > 0);
            Assert.Equal(2, feeQuote.Fees.Length);
            Assert.True(new PublicKey(feeQuote.MinerId).IsValid);
            Assert.True(feeQuote.MiningRates.standard.Bytes > 0);
            Assert.True(feeQuote.MiningRates.standard.Satoshis >= 0);
            Assert.True(feeQuote.RelayRates.standard.Bytes > 0);
            Assert.True(feeQuote.RelayRates.standard.Satoshis >= 0);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("https://merchantapi.matterpool.io")]
//        [InlineData("https://www.ddpurse.com/openapi,561b756d12572020ea9a104c3441b71790acbbce95a6ddbf7e0630971af9424b")]
        // TAAL's GetTransactionStatus fails for older transactions. The error suggests using GetTransaction.
//        [InlineData("https://merchantapi.taal.com")]
        public async Task MapiGetTxStatusTest(string baseUrl) {
            KzMerchantClient.UserAgent = "KzMerchantClientTest";
            var mapi = KzMerchantClient.GetClient(baseUrl);
            var hashTx = "3ea6bb35923dbff216aa11084280e0d6d477d78ed8010edac92c3253b3d79024";
            var ts = await mapi.GetTransactionStatus(hashTx);
            Assert.NotNull(ts);
            Assert.Equal("0.1.0", ts.ApiVersion);
            Assert.True(Math.Abs((ts.Timestamp - DateTime.UtcNow).TotalMinutes) < 1);
            Assert.True(new PublicKey(ts.MinerId).IsValid);
            Assert.Equal("success", ts.ReturnResult);
            Assert.Equal("", ts.ResultDescription);
            Assert.Equal("0000000000000000011e0221844b65bfbbc2599bbd7f71ca0f914a53d90fa8b6", ts.BlockHash);
            Assert.Equal(631498, ts.BlockHeight);
            Assert.True(ts.Confirmations > 90);

            hashTx = "3ea6bb35923dbff200000000000000000077d78ed8010edac92c3253b3d79024";
            ts = await mapi.GetTransactionStatus(hashTx);
            Assert.NotNull(ts);
            Assert.Equal("0.1.0", ts.ApiVersion);
            Assert.True(Math.Abs((ts.Timestamp - DateTime.UtcNow).TotalMinutes) < 1);
            Assert.True(new PublicKey(ts.MinerId).IsValid);
            Assert.Equal("failure", ts.ReturnResult);
            Assert.True(ts.ResultDescription.Length > 0);
            Assert.Null(ts.BlockHash);
            Assert.Null(ts.BlockHeight);
            Assert.True(ts.Confirmations == 0);
        }

        [Theory]
        [InlineData(null, "0.1.0")]
        [InlineData("https://merchantapi.matterpool.io", "0.1.0")]
        //        [InlineData("https://www.ddpurse.com/openapi,561b756d12572020ea9a104c3441b71790acbbce95a6ddbf7e0630971af9424b")]
        [InlineData("https://merchantapi.taal.com", "1.1.0")]
        public async Task MapiPostTxTest(string baseUrl, string version)
        {
            KzMerchantClient.UserAgent = "KzMerchantClientTest";
            var mapi = KzMerchantClient.GetClient(baseUrl);
            // Transaction with lower input value than output value (negative fee...).
            const string tx1 = "0100000001747623f8e6f9b684c2c72d81245d1f1532043088e76ba63805339823e5b16389000000006a47304402204c108078b91ef1f6d2ce154b11bca8c31f6d37dac451a28c07edd7e737efef3802201ecfb09763d64d3ad293eec1c4ecaf0fd45b91dbdf428e422d282b98483300de4121036166800571f944768676842e4d2f8f96825c0f030139b6b78d6c9830de082828ffffffff09f9e15100000000001976a9143e0ea504169d4ef931e913cbbecb3f07b1d4b6f088acf9e15100000000001976a914229db1b4735321f46165ae5837e47dabd064f16e88acf9e15100000000001976a9144a5b03c7eea7b8e6e611559627a56963d514d1ea88ac6e6e5700000000001976a914c042299061557b60e0e5085bee8fadc8d7e5483388acf9e15100000000001976a914633d58a958c54d9858887b0f3aa65be4eb37f07488acf9e15100000000001976a9148d3cf51026f94d03fda5709160c7171b855ba22488ac50c84c00000000001976a9142a03a8943e47cdbd9ba448994e61d237e8d1ac4b88acf9e15100000000001976a914359f98091121e785e6663f10251832d9ae556f8588acf9e15100000000001976a91415a8feff23bfce20f837956c82e1eb1f2457f93488ac00000000";
            var srl = new KzServiceRequestLog();
            var ptr = await mapi.PostTransaction(tx1.HexToBytes(), srl);
            Assert.False(srl.Success);
            Assert.NotNull(ptr);
            Assert.Equal(version, ptr.ApiVersion);
            Assert.True(ptr.currentHighestBlockHeight > 630000);
            Assert.True(new UInt256(ptr.currentHighestBlockHash).ToBigInteger() > 0);
            Assert.True(Math.Abs((ptr.Timestamp - DateTime.UtcNow).TotalMinutes) < 1);
            Assert.True(new PublicKey(ptr.minerId).IsValid);
            Assert.Equal("failure", ptr.returnResult);
            Assert.True(ptr.resultDescription.Length > 0); // e.g. Not enough fees
            Assert.Equal("", ptr.txid); // e.g. Not enough fees
            Assert.Equal(0, ptr.txSecondMempoolExpiry); // e.g. Not enough fees
        }
#if false
        [Theory]
        [InlineData("3ea6bb35923dbff216aa11084280e0d6d477d78ed8010edac92c3253b3d79024")]
        public async Task GetTransactionStatus(string hashTx) {
            KzMerchantClient.UserAgent = "KzMerchantClientTest";
            var mapi = KzMerchantClient.GetClient();
            var ts = await mapi.GetTransactionStatus(hashTx);
            Assert.NotNull(ts);
            Assert.Equal("0.1.0", ts.apiVersion);
            Assert.True(Math.Abs((ts.timestamp - DateTime.UtcNow).TotalMinutes) < 1);
            Assert.Equal("0000000000000000011e0221844b65bfbbc2599bbd7f71ca0f914a53d90fa8b6", ts.blockHash);
            Assert.Equal(631498, ts.blockHeight);
            Assert.True(ts.confirmations > 90);
            Assert.True(new PublicKey(ts.minerId).IsValid);
            Assert.Equal("success", ts.returnResult);
            Assert.Equal("", ts.resultDescription);
        }
#endif
    }
}

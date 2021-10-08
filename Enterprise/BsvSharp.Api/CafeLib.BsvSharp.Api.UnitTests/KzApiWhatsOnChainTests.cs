#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CafeLib.BsvSharp.Api.UnitTests 
{
    public class KzApiWhatsOnChainTests
    {
        private WhatsOnChain.WhatsOnChain Api { get; } = new WhatsOnChain.WhatsOnChain();

        #region Health

        [Fact]
        public async Task GetHealth_Test()
        {
            var health = await Api.GetHealth();
            Assert.True(health.IsSuccessful);
        }

        #endregion

        #region Address

        [Theory]
        [InlineData("1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR", 107297900, 0)]
        public async Task GetAddressBalance_Test(string address, long confirm, long unconfirmed)
        {
            var balance = await Api.GetAddressBalance(address);
            Assert.Equal(confirm, balance.Confirmed);
            Assert.Equal(unconfirmed, balance.Unconfirmed);
        }

        [Fact]
        public async Task GetBulkAddressBalances_Test()
        {
            var addresses = new[]
            {
                "16ZBEb7pp6mx5EAGrdeKivztd5eRJFuvYP",
                "1KGHhLTQaPr4LErrvbAuGE62yPpDoRwrob"
            };

            var balances = await Api.GetBulkAddressBalances(addresses);
            Assert.NotEmpty(balances);
            Assert.Equal(2, balances.Length);
            Assert.Equal(addresses[0], balances.First().Address);
        }

        [Theory]
        [InlineData("16ZqP5Tb22KJuvSAbjNkoiZs13mmRmexZA", "6b22c47e7956e5404e05c3dc87dc9f46e929acfd46c8dd7813a34e1218d2f9d1", 563052)]
        public async Task GetAddressHistory_Test(string address, string firstTxHash, long firstHeight)
        {
            var addressHistory = await Api.GetAddressHistory(address);
            Assert.NotEmpty(addressHistory);
            Assert.Equal(firstTxHash, addressHistory.First().TxHash);
            Assert.Equal(firstHeight, addressHistory.First().Height);
        }

        [Theory]
        [InlineData("1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR", true)]
        public async Task GetAddressInfo_Test(string address, bool isValid)
        {
            var addressInfo = await Api.GetAddressInfo(address);
            Assert.Equal(address, addressInfo.Address);
            Assert.Equal(isValid, addressInfo.IsValid);
        }

        [Theory]
        [InlineData("1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR", 107297900, 0)]
        public async Task GetAddressUtxos_Test(string address, long value, int position)
        {
            var unspentTransactions = await Api.GetUtxosByAddress(address);
            Assert.NotEmpty(unspentTransactions);
            Assert.Equal(value, unspentTransactions.First().Value);
            Assert.Equal(position, unspentTransactions.First().TransactionPosition);
        }

        [Fact]
        public async Task GetBulkAddressUtxos_Test()
        {
            var addresses = new[]
            {
                "1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR",
                "1KGHhLTQaPr4LErrvbAuGE62yPpDoRwrob"
            };

            var utxos = await Api.GetBulkAddressUtxos(addresses);
            Assert.NotEmpty(utxos);
            Assert.Equal(2, utxos.Length);
            Assert.Equal(addresses[0], utxos.First().Address);
            Assert.Equal(107297900, utxos.First().Utxos.First().Value);
        }

        #endregion

        #region Block

        [Theory]
        [InlineData("000000000000000009322213dd454961301f2126b7e73bd01c0bf042641df24c")]
        public async Task GetBlockByHash_Test(string blockHash)
        {
            var block = await Api.GetBlockByHash(blockHash);
            Assert.Equal(blockHash, block.Hash);
        }

        #endregion

        #region Exchange

        [Fact]
        public async Task GetExchangeRate_Test()
        {
            var rate = await Api.GetExchangeRate();
            Assert.True(rate > 0 && rate < 1000000);
        }

        #endregion

        #region Mapi

        [Fact]
        public async Task GetFeeQuotes_Test()
        {
            var quotes = await Api.GetFeeQuotes();
            Assert.NotEmpty(quotes.ProviderQuotes);
            Assert.Contains(quotes.ProviderQuotes, quote => quote.ProviderName == "taal");
        }

        [Theory]
        [InlineData("995ea8d0f752f41cdd99bb9d54cb004709e04c7dc4088bcbbbb9ea5c390a43c3")]
        public async Task GetTxStatus_Test(string txHash)
        {
            var status = await Api.GetTxStatus(txHash);
            Assert.NotNull(status.Payload);
            Assert.Equal("mempool", status.ProviderName);
        }

        #endregion

        #region Mempool

        [Fact]
        public async Task GetMempoolInfo_Test()
        {
            var mempool = await Api.GetMempoolInfo();
            Assert.NotNull(mempool);
            Assert.True(mempool.Bytes > 0);
        }

        [Fact]
        public async Task GetMempoolTransactions_Test()
        {
            var transactions = await Api.GetMempoolTransactions();
            Assert.NotNull(transactions);
            Assert.NotEmpty(transactions);
        }

        #endregion

        #region Script

        [Theory]
        [InlineData("995ea8d0f752f41cdd99bb9d54cb004709e04c7dc4088bcbbbb9ea5c390a43c3", "52dfceb815ad129a0fd946e3d665f44fa61f068135b9f38b05d3c697e11bad48", 620539)]
        public async Task GetScriptHistory_Test(string scriptHash, string firstTxHash, long firstHeight)
        {
            var addressHistory = await Api.GetScriptHistory(scriptHash);
            Assert.NotEmpty(addressHistory);
            Assert.Equal(firstTxHash, addressHistory.First().TxHash);
            Assert.Equal(firstHeight, addressHistory.First().Height);
        }

        #endregion

        #region Transaction

        [Theory]
        [InlineData("c1d32f28baa27a376ba977f6a8de6ce0a87041157cef0274b20bfda2b0d8df96")]
        public async Task GetTransactionByHash_Test(string hash)
        {
            var tx = await Api.GetTransactionsByHash(hash);
            Assert.Equal(hash, tx.Hash);
        }

        #endregion
    }
}

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

        [Fact]
        public async Task GetHealth_Test()
        {
            var health = await Api.GetHealth();
            Assert.True(health.IsSuccessful);
        }

        [Theory]
        [InlineData("1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR", 107297900, 0)]
        public async Task GetAddressBalance_Test(string address, long confirm, long unconfirmed)
        {
            var balance = await Api.GetAddressBalance(address);
            Assert.Equal(confirm, balance.Confirmed);
            Assert.Equal(unconfirmed, balance.Unconfirmed);
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

        [Fact]
        public async Task GetExchangeRate_Test()
        {
            var rate = await Api.GetExchangeRate();
            Assert.True(rate > 0 && rate < 1000000);
        }

        [Fact]
        public async Task GetFeeQuotes_Test()
        {
            var quotes = await Api.GetFeeQuotes();
            Assert.NotEmpty(quotes.ProviderQuotes);
            Assert.Contains(quotes.ProviderQuotes, quote => quote.ProviderName == "taal");
        }

        [Theory]
        [InlineData("995ea8d0f752f41cdd99bb9d54cb004709e04c7dc4088bcbbbb9ea5c390a43c3", "52dfceb815ad129a0fd946e3d665f44fa61f068135b9f38b05d3c697e11bad48", 620539)]
        public async Task GetScriptHistory_Test(string scriptHash, string firstTxHash, long firstHeight)
        {
            var addressHistory = await Api.GetScriptHistory(scriptHash);
            Assert.NotEmpty(addressHistory);
            Assert.Equal(firstTxHash, addressHistory.First().TxHash);
            Assert.Equal(firstHeight, addressHistory.First().Height);
        }

        [Theory]
        [InlineData("c1d32f28baa27a376ba977f6a8de6ce0a87041157cef0274b20bfda2b0d8df96")]
        public async Task GetTransactionByHash_Test(string hash)
        {
            var tx = await Api.GetTransactionsByHash(hash);
            Assert.Equal(hash, tx.Hash);
        }

        [Theory]
        [InlineData("1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR", 107297900, 0)]
        public async Task GetUnspentTransactionsByAddress_Test(string address, long value, int position)
        {
            var unspentTransactions = await Api.GetUnspentTransactionsByAddress(address);
            Assert.NotEmpty(unspentTransactions);
            Assert.Equal(value, unspentTransactions.First().Value );
            Assert.Equal(position, unspentTransactions.First().TransactionPosition);
        }
    }
}

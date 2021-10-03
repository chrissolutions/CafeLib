#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Threading.Tasks;
using Xunit;

namespace CafeLib.BsvSharp.Api.UnitTests 
{
    public class KzApiWhatsOnChainTests
    {
        [Theory]
        [InlineData("1PgZT1K9gKVtoAjCFnmQsviThu7oYDSCTR", 107297900, 0)]
        [InlineData("16ZqP5Tb22KJuvSAbjNkoiZs13mmRmexZA", 0, 0)]
        public async Task GetAddressBalance_Test(string address, long confirm, long unconfirmed)
        {
            var api = new WhatsOnChain.WhatsOnChain();
            var balance = await api.GetAddressBalance(address);
            Assert.Equal(confirm, balance.Confirmed);
            Assert.Equal(unconfirmed, balance.Unconfirmed);
        }

        [Fact]
        public async Task GetExchangeRate_Test()
        {
            var api = new WhatsOnChain.WhatsOnChain();
            var rate = await api.GetExchangeRate();
            Assert.True(rate > 0 && rate < 1000000);
        }

        [Theory]
        [InlineData("c1d32f28baa27a376ba977f6a8de6ce0a87041157cef0274b20bfda2b0d8df96")]
        public async Task GetTransactionByHash(string hash)
        {
            var api = new WhatsOnChain.WhatsOnChain();
            var tx = await api.GetTransactionsByHash(hash);
            Assert.Equal(hash, tx.Hash);
        }

    }
}

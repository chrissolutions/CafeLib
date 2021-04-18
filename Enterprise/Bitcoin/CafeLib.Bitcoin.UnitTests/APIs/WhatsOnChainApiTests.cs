#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.WhatsOnChain;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.APIs {
    public class WhatsOnChainApiTests
    {
        [Fact]
        public async Task GetExchangeRate_Test()
        {
            var api = new WhatsOnChain();
            var rate = await api.GetExchangeRate();
            Assert.True(rate > 0 && rate < 1000000);
        }

        [Theory]
        [InlineData("c1d32f28baa27a376ba977f6a8de6ce0a87041157cef0274b20bfda2b0d8df96")]
        public async Task GetTransactionByHash(string hash)
        {
            var api = new WhatsOnChain();
            var tx = await api.GetTransactionsByHash(hash);
            Assert.Equal(hash, tx.Hash);
        }
    }
}

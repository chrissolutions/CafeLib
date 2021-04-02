#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Threading.Tasks;
using CafeLib.Bitcoin.APIs;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.APIs {

    public class KzApiWhatsOnChainTests {
        [Fact]
        public async Task TestExchangeRate() {
            var api = new KzApiWhatsOnChain();
            var rate = await api.GetExchangeRate();
            Assert.True(rate > 0 && rate < 1000000);
        }
    }
}

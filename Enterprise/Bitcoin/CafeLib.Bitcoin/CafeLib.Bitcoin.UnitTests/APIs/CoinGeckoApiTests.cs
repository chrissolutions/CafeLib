#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Linq;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.CoinGecko;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.APIs {
    public class CoinGeckoApiTests {
        [Fact]
        public async Task GetSupportedCoinsTest() {
            var api = new CoinGecko();
            var coins = await api.GetSupportedCoins();
            Assert.NotNull(coins);
            Assert.True(coins.Any());
        }
    }
}

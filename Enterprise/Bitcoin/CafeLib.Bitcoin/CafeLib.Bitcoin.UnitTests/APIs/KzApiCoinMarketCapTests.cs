#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Threading.Tasks;
using CafeLib.Bitcoin.APIs;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.APIs {
    public class KzApiCoinMarketCapTests {
        [Fact]
        public async Task Test1() {
            var api = new KzApiCoinMarketCap("e80d5567-c5cc-473c-8453-6b3cfcd35be0");
            var json = await api.LatestListings();
        }
    }
}

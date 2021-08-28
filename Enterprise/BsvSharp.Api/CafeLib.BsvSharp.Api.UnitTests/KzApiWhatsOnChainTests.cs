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
        [Fact]
        public async Task TestExchangeRate()
        {
            var api = new WhatsOnChain.WhatsOnChain();
            var rate = await api.GetExchangeRate();
            Assert.True(rate > 0 && rate < 1000000);
        }
    }
}

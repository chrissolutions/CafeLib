using System;
using CafeLib.Cryptography.UnitTests.BsvSharp.Network;
using Xunit;

namespace CafeLib.Cryptography.UnitTests
{
    public class UnitTest
    {
        private static readonly Lazy<IBitcoinNetwork> LazyNetwork = new Lazy<IBitcoinNetwork>(() => new MainNetwork(), true);
        public static readonly IBitcoinNetwork Network = LazyNetwork.Value;

        [Fact]
        public void Test1()
        {

        }
    }
}

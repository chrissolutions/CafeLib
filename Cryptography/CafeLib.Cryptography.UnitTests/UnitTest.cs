using System;
using CafeLib.Cryptography.UnitTests.BsvSharp.Network;

namespace CafeLib.Cryptography.UnitTests
{
    public class UnitTest
    {
        private static readonly Lazy<IBitcoinNetwork> LazyNetwork = new(() => new MainNetwork(), true);
        public static readonly IBitcoinNetwork Network = LazyNetwork.Value;
    }
}

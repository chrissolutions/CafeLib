using CafeLib.Core.Encodings;
using System;
using System.Text;
using Xunit;

namespace CafeLib.Cryptography.UnitTests
{
    public class HashesTests
    {
        [Fact]
        public void Hash256_Test()
        {
            const string text = "Bitcoin protocol is set in stone and there is no need to change it anytime in future as well as most of the global trade financial transactions are possible to be built using the current protocol itself";
            var bytes = Encoding.UTF8.GetBytes(text);
            var shaHash = Hashes.Hash256(bytes);
            var hexStr = new HexEncoder().Encode(shaHash);
            Assert.Equal("9ec3931d0c3da0157f170ebe5158f14a9e0b965ca9697dcff5063d2feb453fd2", hexStr);
        }
    }
}
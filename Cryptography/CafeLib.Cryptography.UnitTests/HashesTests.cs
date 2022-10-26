using System;
using System.Text;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;
using CafeLib.Cryptography.UnitTests.BsvSharp.Encoding;
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

        [Theory]
        [InlineData(0x00000000, 0x00000000, "")]
        [InlineData(0x6a396f08, 0xFBA4C795, "")]
        [InlineData(0x81f16f39, 0xffffffff, "")]
        [InlineData(0x514e28b7, 0x00000000, "00")]
        [InlineData(0xea3f0b17, 0xFBA4C795, "00")]
        [InlineData(0xfd6cf10d, 0x00000000, "ff")]
        [InlineData(0x16c6b7ab, 0x00000000, "0011")]
        [InlineData(0x8eb51c3d, 0x00000000, "001122")]
        [InlineData(0xb4471bf8, 0x00000000, "00112233")]
        [InlineData(0xe2301fa8, 0x00000000, "0011223344")]
        [InlineData(0xfc2e4a15, 0x00000000, "001122334455")]
        [InlineData(0xb074502c, 0x00000000, "00112233445566")]
        [InlineData(0x8034d2a0, 0x00000000, "0011223344556677")]
        [InlineData(0xb4698def, 0x00000000, "001122334455667788")]
        public void MurmurHash_Test(uint expected, uint seed, string data)
        {
            Assert.Equal(expected, Hashes.MurmurHash3(seed, Encoders.Hex.Decode(data)));
        }

        [Fact]
        public void Randomizer_Bytes_Test()
        {
            var entropy = new ByteSpan(new byte[50]);

            for (var i = 0; i < 100; i++)
            {
                var bytes = Randomizer.GetBytes(50);
                Assert.Equal(50, entropy.Length);
                Assert.False(entropy.Data.SequenceCompareTo(bytes) == 0);
                new ByteSpan(bytes).CopyTo(entropy);
            }
        }
    }
}
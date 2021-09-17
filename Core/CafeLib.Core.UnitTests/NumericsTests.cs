using CafeLib.Core.Encodings;
using CafeLib.Core.Numerics;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class NumericsTests
    {
        private readonly Base58Encoder _base58Encoder = new Base58Encoder();
        private readonly HexEncoder _hexEncoder = new HexEncoder();

        [Fact]
        public void UInt256_Creation_Test()
        {
            const string hex = "988119d6cca702beb1748f4eb497e316467f69580ffa125aa8bcb6fb63dce237";

            var uint256 = new UInt256(hex);
            var uint256Reverse = new UInt256(hex, true);

            Assert.Equal(hex, uint256.ToString());
            Assert.Equal(hex, uint256Reverse.ToStringFirstByteFirst());
        }
    }
}

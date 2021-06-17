using CafeLib.BsvSharp.Encoding;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Base58EncodeDecodeViaInstance()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            var bytes = Base58Encoder.Current.Decode(base58);
            var text = Base58Encoder.Current.Encode(bytes);

            Assert.Equal(base58, text);
            Assert.Equal(hex, HexEncoder.Current.Encode(bytes));
        }

        [Fact]
        public void Base58EncodeDecodeViaEncoders()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            var bytes = Encoders.Base58.Decode(base58);
            var text = Encoders.Base58.Encode(bytes);

            Assert.Equal(base58, text);
            Assert.Equal(hex, HexEncoder.EncodeBytes(bytes));
        }

        [Fact]
        public void Base58VerifyHexEncoding()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            Assert.Equal(base58, Encoders.Base58.FromHex(hex));
            Assert.Equal(hex, Encoders.Base58.ToHex(base58));
        }
    }
}
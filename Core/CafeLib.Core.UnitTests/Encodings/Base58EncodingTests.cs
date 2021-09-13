using CafeLib.Core.Encodings;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class Base58EncodingTests
    {
        private readonly Base58Encoder _base58Encoder = new Base58Encoder();
        private readonly HexEncoder _hexEncoder = new HexEncoder();

        [Fact]
        public void Base58EncodeDecodeViaInstance()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            var bytes = _base58Encoder.Decode(base58);
            var text = _base58Encoder.Encode(bytes);

            Assert.Equal(base58, text);
            Assert.Equal(hex, _hexEncoder.Encode(bytes));
        }

        [Fact]
        public void Base58EncodeDecodeViaEncoders()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            var bytes = _base58Encoder.Decode(base58);
            var text = _base58Encoder.Encode(bytes);

            Assert.Equal(base58, text);
            Assert.Equal(hex, _hexEncoder.Encode(bytes));
        }

        [Fact]
        public void Base58VerifyHexEncoding()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            Assert.Equal(base58, _base58Encoder.Encode(_hexEncoder.Decode(hex)));
            Assert.Equal(hex, _hexEncoder.Encode(_base58Encoder.Decode(base58)));
        }


        [Theory]
        [InlineData("", "")]
        [InlineData("61", "2g")]
        [InlineData("626262", "a3gV")]
        [InlineData("636363", "aPEr")]
        [InlineData("73696d706c792061206c6f6e6720737472696e67", "2cFupjhnEsSn59qHXstmK2ffpLv2")]
        [InlineData("00eb15231dfceb60925886b67d065299925915aeb172c06647", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L")]
        [InlineData("516b6fcd0f", "ABnLTmg")]
        [InlineData("bf4f89001e670274dd", "3SEo3LWLoPntC")]
        [InlineData("572e4794", "3EFU7m")]
        [InlineData("ecac89cad93923c02321", "EJDM8drfXA6uyA")]
        [InlineData("10c8511e", "Rt5zm")]
        [InlineData("00000000000000000000", "1111111111")]
        public void Base58TestCases(string hex, string base58)
        {
            var hexValue = _hexEncoder.Decode(hex);
            var bytes = _base58Encoder.Decode(base58);
            Assert.Equal(hexValue, bytes);
            Assert.Equal(base58, _base58Encoder.Encode(bytes));
        }
    }
}

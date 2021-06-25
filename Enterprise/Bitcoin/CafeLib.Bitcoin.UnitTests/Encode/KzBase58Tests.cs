#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Encoding;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Encode
{
    public class KzBase58Tests
    {
        private static readonly HexEncoder Hex = Encoders.Hex;
        private static readonly Base58Encoder Base58 = Encoders.Base58;

        [Fact]
        public void Base58EncodeDecodeViaInstance()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            var bytes = Encoders.Base58.Decode(base58);
            var text = Encoders.Base58.Encode(bytes);

            Assert.Equal(base58, text);
            Assert.Equal(hex, Hex.Encode(bytes));
        }

        [Fact]
        public void Base58EncodeDecodeViaEncoders()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            var bytes = Base58.Decode(base58);
            var text = Base58.Encode(bytes);

            Assert.Equal(base58, text);
            Assert.Equal(hex, Encoders.Hex.Encode(bytes));
        }

        [Fact]
        public void Base58VerifyHexEncoding()
        {
            const string hex = "73696d706c792061206c6f6e6720737472696e67";
            const string base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2";

            Assert.Equal(base58, Base58.Encode(Hex.Decode(hex)));
            Assert.Equal(hex, Hex.Encode(Base58.Decode(base58)));
        }

        [Theory]
        [InlineData(true, "", "")]
        [InlineData(true, "61", "2g")]
        [InlineData(true, "626262", "a3gV")]
        [InlineData(true, "636363", "aPEr")]
        [InlineData(true, "73696d706c792061206c6f6e6720737472696e67", "2cFupjhnEsSn59qHXstmK2ffpLv2")]
        [InlineData(true, "00eb15231dfceb60925886b67d065299925915aeb172c06647", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L")]
        [InlineData(true, "516b6fcd0f", "ABnLTmg")]
        [InlineData(true, "bf4f89001e670274dd", "3SEo3LWLoPntC")]
        [InlineData(true, "572e4794", "3EFU7m")]
        [InlineData(true, "ecac89cad93923c02321", "EJDM8drfXA6uyA")]
        [InlineData(true, "10c8511e", "Rt5zm")]
        [InlineData(true, "00000000000000000000", "1111111111")]
        public void Base58TestCases(bool ok, string hex, string base58)
        {
            var hexValue = Hex.Decode(hex);
            var result = Base58.TryDecode(base58, out var bytes);
            Assert.Equal(result, ok);
            Assert.Equal(hexValue, bytes);
            Assert.Equal(base58, Base58.Encode(bytes));
        }
    }
}

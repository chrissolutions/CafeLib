﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Encoding;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Encode
{
    public class KzBase58Tests
    {
        private static readonly HexEncoder Hex = Encoders.Hex;
        private static readonly Base58Encoder Base58 = Encoders.Base58;

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
            var hexValue = Hex.Decode(hex);
            var bytes = Base58.Decode(base58);
            Assert.Equal(hexValue, bytes);
            Assert.Equal(base58, Base58.Encode(bytes));
        }
    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open Bitcoin software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Units;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Utility
{

    public class KzAmountTests
    {
        [Fact]
        public void ToStringTest()
        {
            var tcs = new[] {
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = 0L, s = " 0.000_000_00" },
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = 1L, s = " 0.000_000_01" },
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = -1L, s = "-0.000_000_01" },
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = 123456789L, s = " 1.234_567_89" },
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = -123456789L, s = "-1.234_567_89" },
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = 2100000000000000L, s = " 21_000_000.000_000_00" },
                new { g = true, units = false, unit = BitcoinUnit.Bitcoin, v = -2100000000000000L, s = "-21_000_000.000_000_00" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = 0L, s = " 0.000_000_00 Bitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = 1L, s = " 0.000_000_01 Bitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = -1L, s = "-0.000_000_01 Bitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = 123456789L, s = " 1.234_567_89 Bitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = -123456789L, s = "-1.234_567_89 Bitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = 2100000000000000L, s = " 21_000_000.000_000_00 Bitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.Bitcoin, v = -2100000000000000L, s = "-21_000_000.000_000_00 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = 0L, s = " 0.00000000 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = 1L, s = " 0.00000001 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = -1L, s = "-0.00000001 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = 123456789L, s = " 1.23456789 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = -123456789L, s = "-1.23456789 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = 2100000000000000L, s = " 21000000.00000000 Bitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.Bitcoin, v = -2100000000000000L, s = "-21000000.00000000 Bitcoin" },

                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = 0L, s = " 0.000_00" },
                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = 1L, s = " 0.000_01" },
                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = -1L, s = "-0.000_01" },
                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = 123456789L, s = " 1_234.567_89" },
                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = -123456789L, s = "-1_234.567_89" },
                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = 2100000000000000L, s = " 21_000_000_000.000_00" },
                new { g = true, units = false, unit = BitcoinUnit.MilliBitcoin, v = -2100000000000000L, s = "-21_000_000_000.000_00" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = 0L, s = " 0.000_00 MilliBitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = 1L, s = " 0.000_01 MilliBitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = -1L, s = "-0.000_01 MilliBitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = 123456789L, s = " 1_234.567_89 MilliBitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = -123456789L, s = "-1_234.567_89 MilliBitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = 2100000000000000L, s = " 21_000_000_000.000_00 MilliBitcoin" },
                new { g = true, units = true, unit = BitcoinUnit.MilliBitcoin, v = -2100000000000000L, s = "-21_000_000_000.000_00 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = 0L, s = " 0.00000 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = 1L, s = " 0.00001 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = -1L, s = "-0.00001 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = 123456789L, s = " 1234.56789 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = -123456789L, s = "-1234.56789 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = 2100000000000000L, s = " 21000000000.00000 MilliBitcoin" },
                new { g = false, units = true, unit = BitcoinUnit.MilliBitcoin, v = -2100000000000000L, s = "-21000000000.00000 MilliBitcoin" },

                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = 0L, s = " 0" },
                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = 1L, s = " 1" },
                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = -1L, s = "-1" },
                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = 123456789L, s = " 123_456_789" },
                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = -123456789L, s = "-123_456_789" },
                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = 2100000000000000L, s = " 2_100_000_000_000_000" },
                new { g = true, units = false, unit = BitcoinUnit.Satoshi, v = -2100000000000000L, s = "-2_100_000_000_000_000" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = 0L, s = " 0 Satoshi" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = 1L, s = " 1 Satoshi" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = -1L, s = "-1 Satoshi" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = 123456789L, s = " 123_456_789 Satoshi" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = -123456789L, s = "-123_456_789 Satoshi" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = 2100000000000000L, s = " 2_100_000_000_000_000 Satoshi" },
                new { g = true, units = true, unit = BitcoinUnit.Satoshi, v = -2100000000000000L, s = "-2_100_000_000_000_000 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = 0L, s = " 0 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = 1L, s = " 1 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = -1L, s = "-1 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = 123456789L, s = " 123456789 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = -123456789L, s = "-123456789 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = 2100000000000000L, s = " 2100000000000000 Satoshi" },
                new { g = false, units = true, unit = BitcoinUnit.Satoshi, v = -2100000000000000L, s = "-2100000000000000 Satoshi" },
            };

            foreach (var tc in tcs) {
                var v = new Amount(tc.v);
                var s = v.ToString(@group: tc.g, units: tc.units, unit: tc.unit);
                Assert.Equal(tc.s, s);
            }
        }
    }
}

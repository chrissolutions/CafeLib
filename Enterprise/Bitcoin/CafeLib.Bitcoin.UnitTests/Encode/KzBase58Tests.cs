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
        private class TestCase { public bool Ok; public string Hex; public string Base58; }

        private readonly TestCase[] _testCases = {
            new TestCase { Ok = true, Hex = "", Base58 = "" },
            new TestCase { Ok = true, Hex = "61", Base58 = "2g" },
            new TestCase { Ok = true, Hex = "626262", Base58 = "a3gV" },
            new TestCase { Ok = true, Hex = "636363", Base58 = "aPEr" },
            new TestCase { Ok = true, Hex = "73696d706c792061206c6f6e6720737472696e67", Base58 = "2cFupjhnEsSn59qHXstmK2ffpLv2" },
            new TestCase { Ok = true, Hex = "00eb15231dfceb60925886b67d065299925915aeb172c06647", Base58 = "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L" },
            new TestCase { Ok = true, Hex = "516b6fcd0f", Base58 = "ABnLTmg" },
            new TestCase { Ok = true, Hex = "bf4f89001e670274dd", Base58 = "3SEo3LWLoPntC" },
            new TestCase { Ok = true, Hex = "572e4794", Base58 = "3EFU7m" },
            new TestCase { Ok = true, Hex = "ecac89cad93923c02321", Base58 = "EJDM8drfXA6uyA" },
            new TestCase { Ok = true, Hex = "10c8511e", Base58 = "Rt5zm" },
            new TestCase { Ok = true, Hex = "00000000000000000000", Base58 = "1111111111" }
        };

        [Fact]
        public void Base58TestCases()
        {
            var h = Encoders.Hex;
            var e = Encoders.Base58;
            var buf = new byte[256];
            foreach (var tc in _testCases) {
                var hex = h.Decode(tc.Hex);
                var span = buf.AsSpan();
                var ok = e.TryDecode(tc.Base58, ref span);
                Assert.Equal(tc.Ok, ok);
                if (ok) {
                    Assert.Equal(hex, span.ToArray());
                    Assert.Equal(tc.Base58, e.Encode(span));
                }
            }
        }
    }
}

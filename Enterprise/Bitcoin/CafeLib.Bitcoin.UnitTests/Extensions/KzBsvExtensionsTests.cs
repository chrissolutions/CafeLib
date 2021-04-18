#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Encoding;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Extensions {
    public class KzBsvExtensionsTests {
        [Fact]
        public void ToInt32() {
            var bytes = new ByteSpan(new byte[] { 0xAC, 0x1E, 0xED, 0x88 });

            var le = Encoders.Endian.LittleEndianInt32(bytes);
            var be = Encoders.Endian.LittleEndianInt32(bytes);

            Assert.Equal(unchecked((Int32)(0x88ED1EAC)), le);
            Assert.Equal(unchecked((Int32)(0xAC1EED88)), be);
        }
    }
}

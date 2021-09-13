using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class EncodingsTests
    {
        [Fact]
        public void Little_Endian_Test()
        {
            var bytes = new ByteSpan(new byte[] {0xAC, 0x1E, 0xED, 0x88});
            var endianEncoder = new EndianEncoder();

            var le = endianEncoder.LittleEndianInt32(bytes);
            Assert.Equal(unchecked((int) (0x88ED1EAC)), le);
        }

        [Fact]
        public void Big_Endian_Test()
        {
            var endianEncoder = new EndianEncoder();
            var bytes = new ByteSpan(new byte[] {0xAC, 0x1E, 0xED, 0x88});

            var be = endianEncoder.BigEndianInt32(bytes);
            Assert.Equal(unchecked((int) (0xAC1EED88)), be);
        }

        [Fact]
        public void Endian_Decode_Encode_Test()
        {
            const string message = "Kilroy was here!";
            var endianEncoder = new EndianEncoder();

            var encodedBytes = endianEncoder.Decode(message);
            var decoded = endianEncoder.Encode(encodedBytes);

            Assert.Equal(message, decoded);
        }

        [Fact]
        public void Hex_Forward_Encode_Decode_Test()
        {
            var hex = new HexEncoder();

            var b0 = new byte[] {0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf0};
            const string s0 = "123456789abcdef0";
            const string s0U = "123456789ABCDEF0";

            Assert.Equal(s0, hex.Encode(b0));
            Assert.Equal(b0, hex.Decode(s0));
            Assert.Equal(b0, hex.Decode(s0U));
        }

        [Fact]
        public void Hex_Reverse_Encode_Decode_Test()
        {
            var hexReverse = new HexReverseEncoder();

            var b0R = new byte[] {0x10, 0x32, 0x54, 0x76, 0x98, 0xba, 0xdc, 0xfe};
            const string s0R = "fedcba9876543210";
            Assert.Equal(s0R, hexReverse.Encode(b0R));
            Assert.Equal(b0R, hexReverse.Decode(s0R));
        }
    }
}
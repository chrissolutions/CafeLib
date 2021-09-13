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
            var bytes = new ByteSpan(new byte[] { 0xAC, 0x1E, 0xED, 0x88 });
            var endianEncoder = new EndianEncoder();

            var le = endianEncoder.LittleEndianInt32(bytes);
            Assert.Equal(unchecked((int)(0x88ED1EAC)), le);
        }

        [Fact]
        public void Big_Endian_Test()
        {
            var endianEncoder = new EndianEncoder();
            var bytes = new ByteSpan(new byte[] { 0xAC, 0x1E, 0xED, 0x88 });

            var be = endianEncoder.BigEndianInt32(bytes);
            Assert.Equal(unchecked((int)(0xAC1EED88)), be);
        }

        [Fact]
        public void Decode_Encode_Test()
        {
            const string message = "Kilroy was here!";
            var endianEncoder = new EndianEncoder();

            var encodedBytes = endianEncoder.Decode(message);
            var decoded = endianEncoder.Encode(encodedBytes);

            Assert.Equal(message, decoded);
        }
    }
}

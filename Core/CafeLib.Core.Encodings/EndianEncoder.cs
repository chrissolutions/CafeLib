using System;
using System.Buffers.Binary;
using CafeLib.Core.Buffers;

namespace CafeLib.Core.Encodings
{
    public class EndianEncoder : IEndianEncoder
    {
        public int LittleEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32LittleEndian(data);
        public int BigEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32BigEndian(data);

        public long LittleEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64LittleEndian(data);
        public long BigEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64BigEndian(data);

        public string Encode(byte[] bytes)
        {
            return BitConverter.IsLittleEndian
                ? System.Text.Encoding.Unicode.GetString(bytes)
                : System.Text.Encoding.BigEndianUnicode.GetString(bytes);
        }

        public byte[] Decode(string source)
        {
            return TryDecode(source, out var bytes)
                ? bytes
                : throw new FormatException(nameof(source));
        }

        public bool TryDecode(string encoded, out byte[] bytes)
        {
            bytes = default;
            try
            {
                bytes = BitConverter.IsLittleEndian
                    ? System.Text.Encoding.Unicode.GetBytes(encoded)
                    : System.Text.Encoding.BigEndianUnicode.GetBytes(encoded);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

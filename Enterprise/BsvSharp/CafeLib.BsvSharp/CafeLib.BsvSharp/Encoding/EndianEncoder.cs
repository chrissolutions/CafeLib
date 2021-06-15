using System;
using System.Buffers.Binary;
using CafeLib.BsvSharp.Buffers;

namespace CafeLib.BsvSharp.Encoding
{
    public class EndianEncoder : Encoder
    {
        public int LittleEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32LittleEndian(data);
        public int BigEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32BigEndian(data);

        public long LittleEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64LittleEndian(data);
        public long BigEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64BigEndian(data);

        public override string Encode(ReadOnlyByteSpan data)
        {
            return BitConverter.IsLittleEndian
                ? System.Text.Encoding.Unicode.GetString(data) 
                : System.Text.Encoding.BigEndianUnicode.GetString(data);
        }

        public override bool TryDecode(string encoded, out byte[] bytes)
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

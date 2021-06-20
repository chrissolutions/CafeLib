using System;
using System.Buffers.Binary;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Core.Support;

namespace CafeLib.Bitcoin.Encoding
{
    public class EndianEncoder : SingletonBase<EndianEncoder>
    {
        public int LittleEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32LittleEndian(data);
        public int BigEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32BigEndian(data);

        public long LittleEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64LittleEndian(data);
        public long BigEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64BigEndian(data);

        public string Encode(ReadOnlyByteSpan data)
        {
            return BitConverter.IsLittleEndian
                ? System.Text.Encoding.Unicode.GetString(data) 
                : System.Text.Encoding.BigEndianUnicode.GetString(data);
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

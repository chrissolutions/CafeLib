using System;
using System.Buffers.Binary;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Core.Support;

namespace CafeLib.Bitcoin.Encoding
{
    public class EndianEncoder : SingletonBase<EndianEncoder>, IEncoder
    {
        public int LittleEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32LittleEndian(data);
        public int BigEndianInt32(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt32BigEndian(data);

        public long LittleEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64LittleEndian(data);
        public long BigEndianInt64(ReadOnlyByteSpan data) => BinaryPrimitives.ReadInt64BigEndian(data);

        public byte[] Decode(string source)
        {
            return BitConverter.IsLittleEndian
                ? System.Text.Encoding.Unicode.GetBytes(source)
                : System.Text.Encoding.BigEndianUnicode.GetBytes(source);
       }

        public string Encode(byte[] bytes)
        {
            return BitConverter.IsLittleEndian
                ? System.Text.Encoding.Unicode.GetString(bytes) 
                : System.Text.Encoding.BigEndianUnicode.GetString(bytes);
        }
    }
}

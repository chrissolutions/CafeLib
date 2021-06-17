using System;
using System.Buffers.Binary;
using CafeLib.Core.Support;

namespace CafeLib.BsvSharp.Encoding
{
    public class EndianEncoder : SingletonBase<EndianEncoder>, IEncoder
    {
        public int LittleEndianInt32(byte[] data) => BinaryPrimitives.ReadInt32LittleEndian(data);
        public int BigEndianInt32(byte[] data) => BinaryPrimitives.ReadInt32BigEndian(data);

        public long LittleEndianInt64(byte[] data) => BinaryPrimitives.ReadInt64LittleEndian(data);
        public long BigEndianInt64(byte[] data) => BinaryPrimitives.ReadInt64BigEndian(data);

        public bool IsBigEndian => !BitConverter.IsLittleEndian;
        public bool IsLittleEndian => BitConverter.IsLittleEndian;

        public string Encode(byte[] source)
        {
            return IsLittleEndian
                ? System.Text.Encoding.Unicode.GetString(source)
                : System.Text.Encoding.BigEndianUnicode.GetString(source);
        }

        public byte[] Decode(string source)
        {
            return IsLittleEndian
                ? System.Text.Encoding.Unicode.GetBytes(source)
                : System.Text.Encoding.BigEndianUnicode.GetBytes(source);
        }
    }
}
using System;

namespace CafeLib.Core.Encodings
{
    public class Utf8Encoder : IEncoder
    {
        public string Encode(byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes);

        public byte[] Decode(string source) => TryDecode(source, out var bytes) ? bytes : throw new FormatException(nameof(source));

        public bool TryDecode(string data, out byte[] bytes)
        {
            try
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(data);
                return true;
            }
            catch
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }
    }
}
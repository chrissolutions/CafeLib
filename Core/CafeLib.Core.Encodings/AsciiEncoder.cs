using System;

namespace CafeLib.Core.Encodings
{
    public class AsciiEncoder : IEncoder
    {
        public string Encode(byte[] bytes) => System.Text.Encoding.ASCII.GetString(bytes);

        public byte[] Decode(string source) => TryDecode(source, out var bytes) ? bytes : throw new FormatException(nameof(source));

        public bool TryDecode(string data, out byte[] bytes)
        {
            try
            {
                bytes = System.Text.Encoding.ASCII.GetBytes(data);
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
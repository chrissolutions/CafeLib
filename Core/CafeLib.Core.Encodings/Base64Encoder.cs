using System;

namespace CafeLib.Core.Encodings
{
    /// <summary>
    /// Base64 Encoder.
    /// </summary>
    public class Base64Encoder : IEncoder
    {
        public string Encode(byte[] bytes) => Convert.ToBase64String(bytes);

        public byte[] Decode(string source) => TryDecode(source, out var bytes) ? bytes : throw new FormatException(nameof(source));

        public bool TryDecode(string data, out byte[] bytes)
        {
            try
            {
                bytes = Convert.FromBase64String(data);
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
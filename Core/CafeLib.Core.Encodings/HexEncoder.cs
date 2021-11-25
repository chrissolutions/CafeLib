using System;
using System.Linq;
using CafeLib.Core.Buffers;

namespace CafeLib.Core.Encodings
{
    /// <summary>
    /// The string begins with the first byte.
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// Character 0 corresponds to the high nibble of the first byte. 
    /// Character 1 corresponds to the low nibble of the first byte. 
    /// </summary>
    public class HexEncoder : IEncoder
    {
        private const int NumericDigits = '9' - '0' + 1;
        private const int AlphaDigits = 'f' - 'a' + 1;
        protected static readonly string[] ByteToChs = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();
        protected static readonly int[] CharToNibbleArray = new int['f' + 1];

        static HexEncoder()
        {
            for (var i = 0; i < 'a'; ++i) CharToNibbleArray[i] = -1;
            for (var i = 0; i < NumericDigits; ++i) CharToNibbleArray[i + '0'] = i;
            for (var i = 0; i < AlphaDigits; ++i)
            {
                CharToNibbleArray[i + 'a'] = i + NumericDigits;
                CharToNibbleArray[i + 'A'] = i + NumericDigits;
            }
        }

        public string Encode(byte[] source) => EncodeSpan(source);

        public byte[] Decode(string source) => TryDecode(source, out var bytes) ? bytes : throw new FormatException(nameof(source));

        public bool TryDecode(string hex, out byte[] bytes)
        {
            hex = hex.StartsWith("0x") ? hex[2..] : hex;
            bytes = new byte[hex.Length / 2];
            var span = bytes.AsSpan();
            return TryDecodeSpan(hex, span);
        }

        public virtual string EncodeSpan(ReadOnlyByteSpan bytes)
        {
            var s = new char[bytes.Length * sizeof(char)];
            var i = 0;
            foreach (var b in bytes)
            {
                var chs = ByteToChs[b];
                s[i++] = chs[0];
                s[i++] = chs[1];
            }
            return new string(s);
        }

        public virtual bool TryDecodeSpan(string hex, ByteSpan bytes)
        {
            ReadOnlyCharSpan chars= hex.StartsWith("0x") ? hex[2..] : hex;
            if (chars.Length % 2 == 1 || chars.Length / 2 > bytes.Length)
                return false;

            if (chars.Length / 2 < bytes.Length)
                bytes[(chars.Length / 2)..].Data.Fill(0);

            for (int i = 0, j = 0; i < chars.Length;)
            {
                var a = CharToNibble(chars[i++]);
                var b = CharToNibble(chars[i++]);
                if (a == -1 || b == -1) return false;
                bytes[j++] = (byte)((a << 4) | b);
            }

            return true;
        }

        protected static int CharToNibble(char c)
        {
            return c > CharToNibbleArray.Length ? -1 : CharToNibbleArray[c];
        }
    }
}

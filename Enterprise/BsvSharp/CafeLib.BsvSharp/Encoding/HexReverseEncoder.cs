using System;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Encoding
{
    /// <summary>
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// The string begins with the last byte.
    /// Character 0 corresponds to the high nibble of the last byte. 
    /// Character 1 corresponds to the low nibble of the last byte. 
    /// </summary>
    public class HexReverseEncoder : HexEncoder
    {
        internal override string EncodeSpan(ReadOnlyByteSpan bytes)
        {
            var s = new char[bytes.Length * 2];
            var i = s.Length;
            foreach (var b in bytes)
            {
                var chs = ByteToChs[b];
                s[--i] = chs[1];
                s[--i] = chs[0];
            }
            return new string(s);
        }

        internal override bool TryDecodeSpan(string hex, ByteSpan bytes)
        {
            if (hex.Length % 2 == 1)
                throw new ArgumentException("Invalid hex bytes string.", nameof(hex));

            if (hex.Length != bytes.Length * 2)
                throw new ArgumentException("Length mismatch.", nameof(bytes));

            for (int i = 0, j = bytes.Length; i < hex.Length;)
            {
                var a = CharToNibble(hex[i++]);
                var b = CharToNibble(hex[i++]);

                if (a == -1 || b == -1) return false;
                bytes[--j] = (byte)((a << 4) | b);
            }

            return true;
        }
    }
}
using System;
using CafeLib.Core.Support;

namespace CafeLib.BsvSharp.Encoding
{
    /// <summary>
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// The string begins with the last byte.
    /// Character 0 corresponds to the high nibble of the last byte. 
    /// Character 1 corresponds to the low nibble of the last byte. 
    /// </summary>
    public class HexReverseEncoder : SingletonBase<HexReverseEncoder>, IEncoder
    {
        public string Encode(byte[] source)
        {
            var s = new char[source.Length * sizeof(char)];
            var i = s.Length;
            foreach (var b in source)
            {
                var chs = HexEncoder.Current.ByteToChs[b];
                s[--i] = chs[1];
                s[--i] = chs[0];
            }

            return new string(s);
        }

        public byte[] Decode(string source)
        {
            if (source.Length % 2 == 1)
                throw new ArgumentException("Invalid hex bytes string.", nameof(source));

            var bytes = new byte[source.Length * sizeof(char)];
            for (int i = 0, j = bytes.Length; i < source.Length;)
            {
                var a = HexEncoder.Current.CharToNibble(source[i++]);
                var b = HexEncoder.Current.CharToNibble(source[i++]);

                if (a == -1 || b == -1) throw new FormatException(nameof(source));
                bytes[--j] = (byte)((a << 4) | b);
            }

            return bytes;
        }
    }
}
using System;
using System.Linq;
using CafeLib.BsvSharp.Buffers;
using CafeLib.Core.Support;

namespace CafeLib.BsvSharp.Encoding
{
    public class HexEncoder : SingletonBase<HexEncoder>, IEncoder
    {
        internal readonly string[] ByteToChs = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();
        internal static readonly int[] CharToNibbleArray = new int['f' + 1];

        private HexEncoder()
        {
            for (var i = 0; i < 'a'; i++) CharToNibbleArray[i] = -1;
            for (var i = 0; i < 10; i++) CharToNibbleArray[i + '0'] = i;
            for (var i = 0; i < 6; i++)
            {
                CharToNibbleArray[i + 'a'] = i + 10;
                CharToNibbleArray[i + 'A'] = i + 10;
            }
        }

        public static string EncodeBytes(byte[] source) => Current.Encode(source);
        public static byte[] DecodeString(string source) => Current.Decode(source);

        public string Encode(byte[] source)
        {
            var s = new char[source.Length * sizeof(char)];
            var i = 0;
            foreach (var b in source)
            {
                var chs = ByteToChs[b];
                s[i++] = chs[0];
                s[i++] = chs[1];
            }
            return new string(s);
        }

        public byte[] Decode(string source)
        {
            var span = new ByteSpan(new byte[source.Length / 2]);

            if (source.Length % 2 == 1 || source.Length / 2 > span.Length) throw new FormatException(nameof(source));

            if (source.Length / 2 < span.Length)
            {
                span[(source.Length / 2)..].Data.Fill(0);
            }

            for (int i = 0, j = 0; i < source.Length;)
            {
                var a = CharToNibble(source[i++]);
                var b = CharToNibble(source[i++]);
                if (a == -1 || b == -1) throw new FormatException(nameof(source));
                span[j++] = (byte)((a << 4) | b);
            }

            return span;
        }

        internal int CharToNibble(char c) => c > CharToNibbleArray.Length ? -1 : CharToNibbleArray[c];
    }
}

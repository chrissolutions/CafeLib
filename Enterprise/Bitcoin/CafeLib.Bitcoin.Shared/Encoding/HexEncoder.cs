#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.Bitcoin.Shared.Buffers;

namespace CafeLib.Bitcoin.Shared.Encoding
{
    /// <summary>
    /// The string begins with the first byte.
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// Character 0 corresponds to the high nibble of the first byte. 
    /// Character 1 corresponds to the low nibble of the first byte. 
    /// </summary>
    public class HexEncoder : Encoder
    {
        protected static readonly string[] ByteToChs = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();
        protected static readonly int[] CharToNibbleArray;

        static HexEncoder()
        {
            CharToNibbleArray = new int['f' + 1];
            for (var i = 0; i < 'a'; i++) CharToNibbleArray[i] = -1;
            for (var i = 0; i < 10; i++) CharToNibbleArray[i + '0'] = i;
            for (var i = 0; i < 6; i++)
            {
                CharToNibbleArray[i + 'a'] = i + 10;
                CharToNibbleArray[i + 'A'] = i + 10;
            }
        }

        public override string Encode(ReadOnlyByteSequence bytes)
        {
            var s = new char[bytes.Length * 2];
            var i = 0;
            foreach (var m in bytes.Data)
            {
                foreach (var b in m.Span)
                {
                    var chs = ByteToChs[b];
                    s[i++] = chs[0];
                    s[i++] = chs[1];
                }
            }
            return new string(s);
        }

        public override string Encode(ReadOnlyByteSpan bytes)
        {
            var s = new char[bytes.Length * 2];
            var i = 0;
            foreach (var b in bytes.Data)
            {
                var chs = ByteToChs[b];
                s[i++] = chs[0];
                s[i++] = chs[1];
            }
            return new string(s);
        }

        protected static int CharToNibble(char c)
        {
            return c > CharToNibbleArray.Length ? -1 : CharToNibbleArray[c];
        }

        public override bool TryDecode(string hex, Span<byte> bytes)
        {
            if (hex.Length % 2 == 1 || hex.Length / 2 > bytes.Length)
                return false;

            if (hex.Length / 2 < bytes.Length)
                bytes[(hex.Length / 2)..].Fill(0);

            for (int i = 0, j = 0; i < hex.Length;)
            {
                var a = CharToNibble(hex[i++]);
                var b = CharToNibble(hex[i++]);
                if (a == -1 || b == -1) goto fail;
                bytes[j++] = (byte)((a << 4) | b);
            }
            return true;
            fail:
            return false;
        }

        public override bool TryDecode(string hex, out byte[] bytes)
        {
            bytes = new byte[hex.Length / 2];
            var span = bytes.AsSpan();
            return TryDecode(hex, span);
        }
    }
}

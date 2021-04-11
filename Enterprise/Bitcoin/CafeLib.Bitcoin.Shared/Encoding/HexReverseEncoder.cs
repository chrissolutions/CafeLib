﻿using System;
using CafeLib.Bitcoin.Shared.Buffers;

namespace CafeLib.Bitcoin.Shared.Encoding
{
    /// <summary>
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// The string begins with the last byte.
    /// Character 0 corresponds to the high nibble of the last byte. 
    /// Character 1 corresponds to the low nibble of the last byte. 
    /// </summary>
    public class HexReverseEncoder : HexEncoder
    {
        public override string Encode(ReadOnlyByteSequence bytes)
        {
            var s = new char[bytes.Length * 2];
            var i = s.Length;
            foreach (var m in bytes.Data)
            {
                foreach (var b in m.Span)
                {
                    var chs = ByteToChs[b];
                    s[--i] = chs[1];
                    s[--i] = chs[0];
                }
            }
            return new string(s);
        }

        public override string Encode(ReadOnlyByteSpan bytes)
        {
            var s = new char[bytes.Length * 2];
            var i = s.Length;
            foreach (var b in bytes.Data)
            {
                var chs = ByteToChs[b];
                s[--i] = chs[1];
                s[--i] = chs[0];
            }
            return new string(s);
        }

        public override bool TryDecode(string hex, Span<byte> bytes)
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
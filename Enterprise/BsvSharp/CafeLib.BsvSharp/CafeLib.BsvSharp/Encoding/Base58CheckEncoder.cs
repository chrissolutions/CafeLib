#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Crypto;

namespace CafeLib.BsvSharp.Encoding
{
    public class Base58CheckEncoder : IEncoder
    {
        /// <summary>
        /// Appends first 4 bytes of double SHA256 hash to bytes before standard Base58 encoding.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string Encode(byte[] bytes)
        {
            var span = new ReadOnlyByteSpan(bytes);
            var checksum = span.Hash256();
            var buf = new byte[span.Length + 4];
            span.CopyTo(buf);
            checksum.Span[..4].CopyTo(buf.AsSpan()[span.Length..]);
            return Encoders.Base58.Encode(buf);
        }

        public byte[] Decode(string source)
        {
            return TryDecode(source, out var bytes) 
                ? bytes 
                : throw new FormatException(nameof(source));
        }

        public bool TryDecode(string encoded, out byte[] bytes)
        {
            if (!Encoders.Base58.TryDecode(encoded, out bytes)) return false;
            var span = bytes.AsSpan();
            var checksum = span[^4..];
            bytes = span[..^4].ToArray();
            var hash = Hashes.Hash256(bytes);
            return checksum.SequenceEqual(hash.Span[..4]);
        }
    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Core.Support;

namespace CafeLib.Bitcoin.Encoding
{
    public class Base58CheckEncoder : SingletonBase<Base58CheckEncoder>
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

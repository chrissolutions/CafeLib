#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Crypto;

namespace CafeLib.Bitcoin.Encoding
{
    public class Base58CheckEncoder : Encoder
    {
        /// <summary>
        /// Appends first 4 bytes of double SHA256 hash to bytes before standard Base58 encoding.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public override string Encode(ReadOnlyByteSpan bytes)
        {
            var checksum = bytes.Hash256();
            var buf = new byte[bytes.Length + 4];
            bytes.CopyTo(buf);
            checksum.Span.Slice(0, 4).CopyTo(buf.AsSpan().Slice(bytes.Length));
            return Encoders.Base58.Encode(buf);
        }

        public override bool TryDecode(string encoded, out byte[] bytes)
        {
            if (!Encoders.Base58.TryDecode(encoded, out bytes)) return false;
            var span = bytes.AsSpan();
            var checksum = span.Slice(span.Length - 4);
            bytes = span.Slice(0, span.Length - 4).ToArray();
            var hash = Hashes.Hash256(bytes);
            return checksum.SequenceEqual(hash.Span.Slice(0, 4));
        }
    }
}

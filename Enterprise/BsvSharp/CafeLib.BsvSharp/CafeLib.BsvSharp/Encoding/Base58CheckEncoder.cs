#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Crypto;
using CafeLib.Core.Support;

namespace CafeLib.BsvSharp.Encoding
{
    public class Base58CheckEncoder : SingletonBase<Base58CheckEncoder>, IEncoder
    {
        public string Encode(ReadOnlyByteSpan data1, ReadOnlyByteSpan data2)
        {
            var bytes = new byte[data1.Length + data2.Length].AsSpan();
            data1.CopyTo(bytes);
            data2.CopyTo(bytes.Slice(data1.Length));
            return Encode(bytes.ToArray());
        }

        /// <summary>
        /// Appends first 4 bytes of double SHA256 hash to bytes before standard Base58 encoding.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string Encode(byte[] source)
        {
            var span = new ReadOnlyByteSpan(source);
            var checksum = span.Hash256();
            var buf = new byte[span.Length + 4];
            span.CopyTo(buf);
            checksum.Span.Slice(0, 4).CopyTo(buf.AsSpan().Slice(span.Length));
            return Encoders.Base58.Encode(buf);
        }

        public byte[] Decode(string source)
        {
            var bytes = Encoders.Base58.Decode(source);
            var span = bytes.AsSpan();
            var checksum = span.Slice(span.Length - 4);
            bytes = span.Slice(0, span.Length - 4).ToArray();
            var hash = Hashes.Hash256(bytes);
            return checksum.SequenceEqual(hash.Span.Slice(0, 4))
                ? bytes
                : throw new FormatException(source);
        }
    }
}

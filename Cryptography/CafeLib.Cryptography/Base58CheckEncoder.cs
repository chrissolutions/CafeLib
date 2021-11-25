using System;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;

namespace CafeLib.Cryptography
{
    public class Base58CheckEncoder : IEncoder
    {
        private static readonly Base58Encoder Base58Encoder = new();

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
            return Base58Encoder.Encode(buf);
        }

        public byte[] Decode(string source)
        {
            return TryDecode(source, out var bytes) 
                ? bytes 
                : throw new FormatException(nameof(source));
        }

        public bool TryDecode(string encoded, out byte[] bytes)
        {
            if (!Base58Encoder.TryDecode(encoded, out bytes)) return false;
            var span = bytes.AsSpan();
            var checksum = span[^4..];
            bytes = span[..^4].ToArray();
            var hash = ((ReadOnlyByteSpan)bytes).Hash256();
            return checksum.SequenceEqual(hash.Span[..4]);
        }
    }
}

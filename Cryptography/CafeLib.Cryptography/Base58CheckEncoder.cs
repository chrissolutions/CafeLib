using System;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;

namespace CafeLib.Cryptography
{
    public class Base58CheckEncoder : IEncoder
    {
        private const int ChecksumSize = 4;
        private static readonly Base58Encoder Base58Encoder = new();


        /// <summary>
        /// Appends first 4 bytes of double SHA256 hash as checksum to bytes before standard Base58 encoding.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string Encode(byte[] bytes)
        {
            var span = new ReadOnlyByteSpan(bytes);
            var checksum = span.Hash256();
            var buf = new byte[span.Length + ChecksumSize];
            span.CopyTo(buf);
            checksum.Span[..ChecksumSize].CopyTo(buf.AsSpan()[span.Length..]);
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
            var checksum = span[^ChecksumSize..];
            bytes = span[..^ChecksumSize].ToArray();
            var hash = ((ReadOnlyByteSpan)bytes).Hash256();
            return checksum.SequenceEqual(hash.Span[..ChecksumSize]);
        }
    }
}

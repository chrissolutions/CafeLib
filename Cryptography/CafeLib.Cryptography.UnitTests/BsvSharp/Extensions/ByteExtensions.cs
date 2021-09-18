using System;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Buffers;
using CafeLib.Core.Numerics;

namespace CafeLib.Cryptography.UnitTests.BsvSharp.Extensions
{
    public static class ByteExtensions
    {
        public static ByteSpan Slice(this byte[] a, int start) => a.AsSpan().Slice(start);
        public static ByteSpan Slice(this byte[] a, int start, int length) => a.AsSpan().Slice(start, length);

        public static int GetHashCodeOfValues(this IEnumerable<byte> a)
        {
            return a?.Aggregate(17, (current, b) => current * 31 + b) ?? 0;
        }

        public static UInt160 Hash160(this byte[] data)
        {
            var hash = new UInt160();
            new ReadOnlyByteSpan(data).Sha1(hash.Span);
            return hash;
        }

        public static UInt256 Hash256(this byte[] data)
        {
            var hash = new UInt256();
            new ReadOnlyByteSpan(data).Sha256(hash.Span);
            return hash;
        }
    }
}

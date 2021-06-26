using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Crypto;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Extensions
{
    public static class ByteExtensions
    {
        public static int AggregateHashCode(this IEnumerable<byte> bytes) => bytes?.Aggregate(17, (current, b) => current * 31 + b) ?? 0;

        public static ByteSpan Slice(this byte[] a, int start) => a.AsSpan().Slice(start);
        public static ByteSpan Slice(this byte[] a, int start, int length) => a.AsSpan().Slice(start, length);

        public static byte[] Duplicate(this byte[] source)
        {
            var dup = new byte[source.Length];
            Buffer.BlockCopy(source, 0, dup, 0, source.Length);
            return dup;
        }

        /// <summary>
        /// Copy to byte array from another byte array.
        /// </summary>
        /// <param name="source">source byte array</param>
        /// <param name="destination">destination byte array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this byte[] source, ref byte[] destination)
        {
            ((ReadOnlyByteSpan)source).CopyTo(destination);
        }

        public static UInt160 Hash160(this byte[] data)
        {
            var hash = new UInt160();
            new ReadOnlyByteSequence(data).Sha1(hash);
            return hash;
        }

        public static UInt256 Hash256(this byte[] data)
        {
            var hash = new UInt256();
            new ReadOnlyByteSequence(data).Sha256(hash);
            return hash;
        }
    }
}

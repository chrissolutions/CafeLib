using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CafeLib.Bitcoin.Buffers;

namespace CafeLib.Bitcoin.Extensions
{
    public static class ByteExtensions
    {
        public static int AggregateHashCode(this IEnumerable<byte> bytes) => bytes?.Aggregate(17, (current, b) => current * 31 + b) ?? 0;

        public static Span<byte> Slice(this byte[] a, int start) => a.AsSpan().Slice(start);
        public static ByteSpan Slice(this byte[] a, int start, int length) => a.AsSpan().Slice(start, length);

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
    }
}

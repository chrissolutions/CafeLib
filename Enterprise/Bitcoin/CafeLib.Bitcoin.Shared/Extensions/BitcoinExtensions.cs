using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using CafeLib.Bitcoin.Shared.Buffers;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class BitcoinExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyByteSpan ToSpan(ref this ReadOnlySequence<byte> sequence)
        {
            return sequence.IsSingleSegment
                ? new SequenceReader<byte>(sequence).UnreadSpan
                : sequence.ToArray();
        }

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        public static ReadOnlyByteSpan AsReadOnlySpan(this ref Int32 i, bool bigEndian = false)
        {
            if (BitConverter.IsLittleEndian == !bigEndian)
                return ToSpan(ref i);

            var bytes = ToSpan(ref i);
            Array.Reverse(bytes);
            return bytes;

            static ReadOnlyByteSpan ToSpan(ref Int32 i)
            {
                unsafe
                {
                    fixed (Int32* p = &i)
                    {
                        var pb = (byte*)p;
                        var span = new Span<byte>(pb, 4);
                        return span;
                    }
                }
            }
        }
    }
}

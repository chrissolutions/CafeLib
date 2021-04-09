using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class BitcoinExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ToSpan(ref this ReadOnlySequence<byte> sequence)
        {
            return sequence.IsSingleSegment
                ? new SequenceReader<byte>(sequence).UnreadSpan
                : sequence.ToArray();
        }
    }
}

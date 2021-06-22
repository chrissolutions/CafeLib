using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CafeLib.Bitcoin.Buffers
{
    public readonly struct ReadOnlyByteSequence
    {
        public static readonly ReadOnlyByteSequence Empty = new ReadOnlyByteSequence(Array.Empty<byte>());

        public ReadOnlySequence<byte> Data { get; }

        public ReadOnlyByteSequence(byte[] data)
        {
            Data = new ReadOnlySequence<byte>(data ?? Array.Empty<byte>());
        }

        public ReadOnlyByteSequence(ReadOnlySequence<byte> data)
        {
            Data = data;
        }

        public bool IsEmpty => Data.IsEmpty;

        public long Length => Data.Length;

        public byte[] ToArray() => Data.ToArray();
        public ReadOnlyByteSequence Slice(SequencePosition start, SequencePosition end) => Data.Slice(start, end);
        public ReadOnlyByteSequence Slice(SequencePosition start, int length) => Data.Slice(start, length);
        public ReadOnlyByteSequence Slice(SequencePosition start, long length) => Data.Slice(start, length);
        public ReadOnlyByteSequence Slice(int start, SequencePosition end) => Data.Slice(start, end);
        //public ReadOnlyByteSequence Slice(int start , int length) => Data.Slice(start, length);
        public ReadOnlyByteSequence Slice(int start) => Data.Slice(start);
        public ReadOnlyByteSequence Slice(long start) => Data.Slice(start);
        public ByteSpan CopyTo(ByteSpan destination)
        {
            Data.CopyTo(destination);
            return destination;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public ref struct Enumerator
        {
            private ReadOnlySequence<byte>.Enumerator _enumerator;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="span">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlyByteSequence span)
            {
                _enumerator = span.Data.GetEnumerator();
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _enumerator.MoveNext();

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public ReadOnlyByteMemory Current => _enumerator.Current;
        }

        public static implicit operator ReadOnlySequence<byte>(ReadOnlyByteSequence rhs) => rhs.Data;
        public static implicit operator ReadOnlyByteSequence(ReadOnlySequence<byte> rhs) => new ReadOnlyByteSequence(rhs);

        public static explicit operator byte[](ReadOnlyByteSequence rhs) => rhs.ToArray();
        public static implicit operator ReadOnlyByteSequence(byte[] rhs) => new ReadOnlyByteSequence(rhs);

        public static implicit operator ReadOnlyByteSpan(ReadOnlyByteSequence rhs) => new ReadOnlyByteSpan(rhs);
        public static explicit operator ReadOnlyByteSequence(ReadOnlyByteSpan rhs) => new ReadOnlyByteSequence(rhs);

        public override int GetHashCode() => Data.GetHashCode();

        public override bool Equals(object obj) => obj is ReadOnlyByteSequence type && this == type;
        public bool Equals(ReadOnlyByteSequence rhs) => ((ReadOnlyByteSpan)this).Data.SequenceEqual((ReadOnlyByteSpan)rhs);

        public static bool operator ==(ReadOnlyByteSequence x, ReadOnlyByteSequence y) => x.Equals(y);
        public static bool operator !=(ReadOnlyByteSequence x, ReadOnlyByteSequence y) => !(x == y);


        /// <summary>
        /// Run down both sequences as long as the bytes are equal.
        /// If we've run out of a bytes, return -1, a is less than b.
        /// If we've run out of b bytes, return 1, a is greater than b.
        /// If both are simultaneously out, they are equal, return 0.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ReadOnlyByteSequence other)
        {
            var ae = GetEnumerator();
            var be = other.GetEnumerator();
            var aok = ae.MoveNext();
            var bok = be.MoveNext();
            var ai = -1;
            var bi = -1;
            var aSpan = ReadOnlyByteSpan.Empty;
            var bSpan = ReadOnlyByteSpan.Empty;
            while (aok && bok)
            {
                if (ai == -1) { aSpan = ae.Current.Data.Span; ai = 0; }
                if (bi == -1) { bSpan = be.Current.Data.Span; bi = 0; }
                if (ai >= aSpan.Length) { ai = -1; aok = ae.MoveNext(); }
                if (bi >= bSpan.Length) { bi = -1; bok = ae.MoveNext(); }
                if (ai == -1 || bi == -1) continue;
                if (aSpan[ai++] != bSpan[bi++]) break;
            }

            return aok
                ? 1
                : bok
                    ? -1
                    : 0;
        }

        /// <summary>
        /// Returns true if sequence starts with other sequence, or if other sequence length is zero.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool StartsWith(ReadOnlyByteSequence other)
        {
            var s = Data;
            var o = other;
            var oLen = o.Length;

            if (oLen > s.Length) return false;

            while (oLen > 0)
            {
                var sMem = Data.First;
                var oMem = other.Data.First;
                var len = Math.Min(sMem.Length, oMem.Length);
                if (!sMem.Span.Slice(0, len).SequenceEqual(oMem.Span.Slice(0, len))) return false;
                s = s.Slice(len);
                o = o.Data.Slice(len);
                oLen = o.Length;
            }
            return true;
        }
        public ReadOnlyByteSequence RemoveSlice(long start, long end)
            => RemoveSlice(Data.GetPosition(start), Data.GetPosition(end));



        /// <summary>
        /// Returns a new ReadOnlySequence with a slice removed.
        /// </summary>
        /// <param name="start">Start of slice to remove.</param>
        /// <param name="end">End of slice to remove</param>
        /// <returns></returns>
        public ReadOnlySequence<byte> RemoveSlice(SequencePosition start, SequencePosition end)
        {
            var before = Data.Slice(Data.Start, start);
            var after = Data.Slice(end, Data.End);

            if (before.Length == 0)
                return after;

            if (after.Length == 0)
                return before;

            // Join before and after sequences.

            var typeBefore = GetSequenceType(ref before);
            var typeAfter = GetSequenceType(ref after);

            if (typeBefore != typeAfter)
                throw new InvalidOperationException();

            var first = (ByteSequenceSegment)null;
            var last = (ByteSequenceSegment)null;
            switch (typeBefore)
            {
                case SequenceType.Segment:
                    foreach (var m in before)
                    {
                        if (last == null)
                            last = first = new ByteSequenceSegment(m);
                        else
                            last = last.Append(m);
                    }
                    foreach (var m in after)
                    {
                        last = last?.Append(m);
                    }
                    break;

                case SequenceType.MemoryManager:
                case SequenceType.Array:
                case SequenceType.String:
                    first = new ByteSequenceSegment(before.First);
                    last = first.Append(after.First);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return new ReadOnlySequence<byte>(first, 0, last, last?.Memory.Length ?? -1);
        }

        private static SequenceType GetSequenceType(ref ReadOnlySequence<byte> sequence)
        {
            var startIndex = sequence.Start.GetInteger();
            var endIndex = sequence.End.GetInteger();

            SequenceType type;
            if (startIndex >= 0)
                type = endIndex >= 0 ? SequenceType.Segment : SequenceType.Array;
            else if (endIndex >= 0)
                type = SequenceType.MemoryManager;
            else
                type = SequenceType.String;
            return type;
        }


        /// <summary>
        /// Forms a slice out of the current <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items.
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The length of the slice.</param>
        /// <returns>A slice that consists of <paramref name="length" /> elements from the current instance starting at index <paramref name="start" />.</returns>
        public ReadOnlyByteSpan Slice(int start, int length) => Slice((long)start, length);


        public ReadOnlyByteSpan Slice(long start, int length)
        {
            var slice = Data.Slice(start, length);
            return slice.IsSingleSegment ? (ReadOnlyByteSpan)slice.FirstSpan : new ReadOnlyByteSpan(slice.ToArray());
        }

        /// <summary>
        /// Forms a slice out of the current <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/> and ending at <paramref name="end"/> (exclusive).
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="end">The ending (exclusive) <see cref="SequencePosition"/> of the slice.</param>
        /// <returns>A slice that consists of items from the <paramref name="start" /> index to, but not including, the <paramref name="end" /> sequence position in the current read-only sequence.</returns>
        //public ReadOnlySequence<T> Slice(int start, SequencePosition end) => Slice((long)start, end);

        /// <summary>
        /// Forms a slice out of the current <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items.
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice.</param>
        /// <returns>A slice that consists of <paramref name="length" /> elements from the current instance starting at sequence position <paramref name="start" />.</returns>
        //public ReadOnlySequence<T> Slice(SequencePosition start, int length) => Slice(start, (long)length);

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (exclusive).
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="end">The ending (exclusive) <see cref="SequencePosition"/> of the slice.</param>
        /// <returns>A slice that consists of items from the <paramref name="start" /> sequence position to, but not including, the <paramref name="end" /> sequence position in the current read-only sequence.</returns>
        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        ////public ReadOnlySequence<T> Slice(SequencePosition start, SequencePosition end)
        ////{
        ////    BoundsCheck((uint)GetIndex(start), start.GetObject(), (uint)GetIndex(end), end.GetObject());
        ////    return SliceImpl(start, end);
        ////}
    }
}

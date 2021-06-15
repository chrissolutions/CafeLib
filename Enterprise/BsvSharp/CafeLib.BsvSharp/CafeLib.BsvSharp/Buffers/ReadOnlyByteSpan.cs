using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CafeLib.BsvSharp.Buffers
{
    public readonly ref struct ReadOnlyByteSpan
    {
        public ReadOnlySpan<byte> Data { get; }

        public ReadOnlyByteSpan(byte[] data)
        {
            Data = data ?? Array.Empty<byte>();
        }

        public ReadOnlyByteSpan(ReadOnlySpan<byte> data)
        {
            Data = data;
        }

        public ReadOnlyByteSpan(ReadOnlyByteSequence data)
        {
            Data = data.Data.IsSingleSegment
                ? new SequenceReader<byte>(data.Data).UnreadSpan
                : data.Data.ToArray();
        }

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ReadOnlyByteSpan Slice(int start) => Data[start..];
        public ReadOnlyByteSpan Slice(int start, int length) => Data.Slice(start, length);

        public byte[] ToArray() => Data.ToArray();

        public ByteSpan CopyTo(ByteSpan destination)
        {
            Data.CopyTo(destination);
            return destination;
        }

        public int SequenceCompareTo(ReadOnlyByteSpan target) => Data.SequenceCompareTo(target);

        public Enumerator GetEnumerator() => new Enumerator(this);

        public ref struct Enumerator
        {
            private ReadOnlySpan<byte>.Enumerator _enumerator;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="span">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlyByteSpan span)
            {
                _enumerator = span.Data.GetEnumerator();
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _enumerator.MoveNext();

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public byte Current => _enumerator.Current;
        }

        public byte this[int index] => Data[index];
        public ReadOnlyByteSpan this[Range range] => Data[range];

        public static ReadOnlyByteSpan Empty => default;

        public static implicit operator ByteSpan(ReadOnlyByteSpan rhs) => rhs.Data;

        public static implicit operator ReadOnlySpan<byte>(ReadOnlyByteSpan rhs) => rhs.Data;
        public static implicit operator ReadOnlyByteSpan(ReadOnlySpan<byte> rhs) => new ReadOnlyByteSpan(rhs);

        public static implicit operator Span<byte>(ReadOnlyByteSpan rhs) => new Span<byte>(rhs.Data.ToArray());
        public static implicit operator ReadOnlyByteSpan(Span<byte> rhs) => new ReadOnlyByteSpan(rhs);

        public static implicit operator byte[](ReadOnlyByteSpan rhs) => rhs.Data.ToArray();
        public static implicit operator ReadOnlyByteSpan(byte[] rhs) => new ReadOnlyByteSpan(rhs);
    }
}

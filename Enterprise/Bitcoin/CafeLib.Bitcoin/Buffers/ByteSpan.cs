using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CafeLib.Bitcoin.Buffers
{
    public readonly ref struct ByteSpan
    {
        public Span<byte> Data { get; }

        public ByteSpan(Span<byte> data)
        {
            Data = data;
        }

        public ByteSpan(byte[] data)
        {
            Data = data ?? Array.Empty<byte>();
        }

        public byte this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public ByteSpan this[Range range] => Data[range];

        public ByteSpan Reverse()
        {
             Data.Reverse();
            return this;
        }

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ByteSpan Slice(int start) => Data[start..];
        public ByteSpan Slice(int start, int length) => Data.Slice(start, length);

        public byte[] ToArray() => Data.ToArray();

        public Enumerator GetEnumerator() => new Enumerator(this);

        public ref struct Enumerator
        {
            private Span<byte>.Enumerator _enumerator;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="span">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ByteSpan span)
            {
                _enumerator = span.Data.GetEnumerator();
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _enumerator.MoveNext();

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public byte Current => _enumerator.Current;
        }

        public ByteSpan CopyTo(ByteSpan destination)
        {
            Data.CopyTo(destination);
            return destination;
        }

        public static ByteSpan Empty => new ByteSpan();

        public static implicit operator ByteSpan(byte[] rhs) => new ByteSpan(rhs);
        public static implicit operator byte[](ByteSpan rhs) => rhs.Data.ToArray();

        public static implicit operator ByteSpan(Span<byte> rhs) => new ByteSpan(rhs);
        public static implicit operator Span<byte>(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlySpan<byte> rhs) => new ByteSpan(rhs.ToArray());
        public static implicit operator ReadOnlySpan<byte>(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlyByteSpan rhs) => new ByteSpan(rhs.ToArray());
        public static implicit operator ReadOnlyByteSpan(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlyByteSequence rhs) => new ByteSpan(rhs.Data.ToArray());
        public static implicit operator ReadOnlyByteSequence(ByteSpan rhs) => new ReadOnlyByteSequence(rhs);
    }
}

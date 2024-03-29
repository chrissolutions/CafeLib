﻿using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CafeLib.Core.Buffers
{
    public readonly ref struct ByteSpan
    {
        public Span<byte> Data { get; }

        public ByteSpan(Span<byte> data)
        {
            Data = data;
        }

        public ByteSpan(byte[] data = null)
            : this(new Span<byte>(data ?? Array.Empty<byte>()))
        {
        }

        public byte this[Index index]
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

        public Enumerator GetEnumerator() => new(this);

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

        public static ByteSpan Empty => new();

        public static implicit operator ByteSpan(byte[] rhs) => new(rhs);
        public static implicit operator byte[](ByteSpan rhs) => rhs.Data.ToArray();

        public static implicit operator ByteSpan(Span<byte> rhs) => new(rhs);
        public static implicit operator Span<byte>(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlySpan<byte> rhs) => new(rhs.ToArray());
        public static implicit operator ReadOnlySpan<byte>(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlyByteSpan rhs) => new(rhs.ToArray());
        public static implicit operator ReadOnlyByteSpan(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlyByteSequence rhs) => new(rhs.Data.ToArray());
        public static implicit operator ReadOnlyByteSequence(ByteSpan rhs) => new(rhs);

        public static ByteSpan operator +(ByteSpan span1, ByteSpan span2)
        {
            return Concat(span1, span2);
        }

        public static ByteSpan operator +(ByteSpan span1, ReadOnlyByteSpan span2)
        {
            return Concat(span1, span2.Data);
        }

        public static ByteSpan Concat(ByteSpan span1, ByteSpan span2)
        {
            var span = new ByteSpan(new byte[span1.Length + span2.Length]);
            span1.CopyTo(span);
            span2.CopyTo(span[span1.Length..]);
            return span;
        }
    }
}

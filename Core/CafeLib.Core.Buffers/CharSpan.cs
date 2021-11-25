using System;
using System.Runtime.CompilerServices;

namespace CafeLib.Core.Buffers
{
    public readonly ref struct CharSpan
    {
        public Span<char> Data { get; }

        public CharSpan(Span<char> data)
        {
            Data = data;
        }

        public CharSpan(char[] data)
        {
            Data = data ?? Array.Empty<char>();
        }

        public CharSpan(string data)
        {
            Data = data?.AsSpan().ToArray() ?? Array.Empty<char>();
        }

        public char this[Index index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public CharSpan this[Range range] => Data[range];

        public CharSpan Reverse()
        {
            Data.Reverse();
            return this;
        }

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public CharSpan Slice(int start) => Data[start..];
        public CharSpan Slice(int start, int length) => Data.Slice(start, length);

        public char[] ToArray() => Data.ToArray();

        public Enumerator GetEnumerator() => new(this);

        public ref struct Enumerator
        {
            private Span<char>.Enumerator _enumerator;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="span">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(CharSpan span)
            {
                _enumerator = span.Data.GetEnumerator();
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _enumerator.MoveNext();

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public char Current => _enumerator.Current;
        }

        public CharSpan CopyTo(CharSpan destination)
        {
            Data.CopyTo(destination);
            return destination;
        }

        public static CharSpan Empty => new();

        public static implicit operator CharSpan(char[] rhs) => new(rhs);
        public static implicit operator char[](CharSpan rhs) => rhs.Data.ToArray();

        public static implicit operator CharSpan(Span<char> rhs) => new(rhs);
        public static implicit operator Span<char>(CharSpan rhs) => rhs.Data;

        public static implicit operator CharSpan(ReadOnlySpan<char> rhs) => new(rhs.ToArray());
        public static implicit operator ReadOnlySpan<char>(CharSpan rhs) => rhs.Data;

        public static implicit operator CharSpan(ReadOnlyCharSpan rhs) => new(rhs.ToArray());
        public static implicit operator ReadOnlyCharSpan(CharSpan rhs) => rhs.Data;
    }
}

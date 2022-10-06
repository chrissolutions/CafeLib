using System;
using System.Runtime.CompilerServices;

namespace CafeLib.Core.Buffers
{
    public readonly ref struct ReadOnlyCharSpan
    {
        public ReadOnlySpan<char> Data { get; }

        public ReadOnlyCharSpan(ReadOnlySpan<char> data)
        {
            Data = data;
        }

        public ReadOnlyCharSpan(string str = null)
            : this((str ?? "").AsSpan())
        {
        }

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ReadOnlyCharSpan Slice(int start) => Data[start..];
        public ReadOnlyCharSpan Slice(int start, int length) => Data[start..(start+length)];

        public char[] ToArray() => Data.ToArray();
        public override string ToString() => Data.ToString();

        public CharSpan CopyTo(CharSpan destination)
        {
            Data.CopyTo(destination);
            return destination;
        }

        public int SequenceCompareTo(ReadOnlyCharSpan target) => Data.SequenceCompareTo(target);

        public Enumerator GetEnumerator() => new(this);

        public ref struct Enumerator
        {
            private ReadOnlySpan<char>.Enumerator _enumerator;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="span">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlyCharSpan span)
            {
                _enumerator = span.Data.GetEnumerator();
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _enumerator.MoveNext();

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public char Current => _enumerator.Current;
        }

        public char this[Index index] => Data[index];
        public ReadOnlyCharSpan this[Range range] => Data[range];

        public static ReadOnlyCharSpan Empty => default;

        public static implicit operator ReadOnlySpan<char>(ReadOnlyCharSpan rhs) => rhs.Data;
        public static implicit operator ReadOnlyCharSpan(ReadOnlySpan<char> rhs) => new(rhs);

        public static implicit operator CharSpan(ReadOnlyCharSpan rhs) => rhs.Data;
        public static implicit operator ReadOnlyCharSpan(Span<char> rhs) => new(rhs);

        public static implicit operator char[](ReadOnlyCharSpan rhs) => rhs.Data.ToArray();
        public static implicit operator ReadOnlyCharSpan(char[] rhs) => new(rhs);
        
        public static implicit operator string(ReadOnlyCharSpan rhs) => rhs.Data.ToString();
        public static implicit operator ReadOnlyCharSpan(string rhs) => new(rhs);


        public static ReadOnlyCharSpan operator +(ReadOnlyCharSpan span1, ReadOnlyCharSpan span2)
        {
            return Concat(span1, span2);
        }

        public static ReadOnlyCharSpan operator +(ReadOnlyCharSpan span1, CharSpan span2)
        {
            return Concat(span1, span2.Data);
        }

        public static ReadOnlyCharSpan Concat(ReadOnlyCharSpan span1, ReadOnlyCharSpan span2)
        {
            var span = new CharSpan(new char[span1.Length + span2.Length]);
            span1.CopyTo(span);
            span2.CopyTo(span[span1.Length..]);
            return span;
        }
    }
}

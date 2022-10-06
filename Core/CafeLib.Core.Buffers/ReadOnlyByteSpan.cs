using System;
using System.Runtime.CompilerServices;

namespace CafeLib.Core.Buffers
{
    public readonly ref struct ReadOnlyByteSpan
    {
        public ReadOnlySpan<byte> Data { get; }

        public ReadOnlyByteSpan(ReadOnlySpan<byte> data)
        {
            Data = data;
        }
        
        public ReadOnlyByteSpan(byte[] data = null)
            : this(new ReadOnlySpan<byte>(data ?? Array.Empty<byte>()))
        {
        }

        public ReadOnlyByteSpan(ReadOnlyByteSequence data)
        {
            Data = data.ToSpan();
        }

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ReadOnlyByteSpan Slice(int start) => Data[start..];
        public ReadOnlyByteSpan Slice(int start, int length) => Data[start..(start+length)];

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

        public byte this[Index index] => Data[index];
        public ReadOnlyByteSpan this[Range range] => Data[range];

        public static ReadOnlyByteSpan Empty => default;

        public static implicit operator ByteSpan(ReadOnlyByteSpan rhs) => rhs.Data;

        public static implicit operator ReadOnlySpan<byte>(ReadOnlyByteSpan rhs) => rhs.Data;
        public static implicit operator ReadOnlyByteSpan(ReadOnlySpan<byte> rhs) => new ReadOnlyByteSpan(                                                                                                                                                                                                                                    rhs);

        public static implicit operator Span<byte>(ReadOnlyByteSpan rhs) => new(rhs.Data.ToArray());
        public static implicit operator ReadOnlyByteSpan(Span<byte> rhs) => new(rhs);

        public static implicit operator byte[](ReadOnlyByteSpan rhs) => rhs.Data.ToArray();
        public static implicit operator ReadOnlyByteSpan(byte[] rhs) => new(rhs);

        public static ReadOnlyByteSpan operator +(ReadOnlyByteSpan span1, ReadOnlyByteSpan span2)
        {
            return Concat(span1, span2);
        }

        public static ReadOnlyByteSpan Concat(ReadOnlyByteSpan span1, ReadOnlyByteSpan span2)
        {
            var span = new ByteSpan(new byte[span1.Length + span2.Length]);
            span1.CopyTo(span);
            span2.CopyTo(span[span1.Length..]);
            return span;
        }
    }
}

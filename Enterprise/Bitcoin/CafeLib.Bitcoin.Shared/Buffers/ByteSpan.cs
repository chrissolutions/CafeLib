using System;
using System.Buffers;

namespace CafeLib.Bitcoin.Shared.Buffers
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
            Data = data;
        }

        public byte this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public ByteSpan this[Range range] => Data[range];

        public void Reverse() => Data.Reverse();

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ByteSpan Slice(int start) => Data[start..];
        public ByteSpan Slice(int start, int length) => Data.Slice(start, length);

        public void CopyTo(ByteSpan destination) => Data.CopyTo(destination);

        public static implicit operator ReadOnlyByteSpan(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlySpan<byte> rhs) => new ByteSpan(rhs.ToArray());
        public static implicit operator ReadOnlySpan<byte>(ByteSpan rhs) => rhs.Data;

        public static implicit operator ByteSpan(ReadOnlySequence<byte> rhs) => new ByteSpan(rhs.ToArray());
        public static implicit operator ReadOnlySequence<byte>(ByteSpan rhs) => new ReadOnlySequence<byte>(rhs);

        public static implicit operator ByteSpan(Span<byte> rhs) => new ByteSpan(rhs);
        public static implicit operator Span<byte>(ByteSpan rhs) => new Span<byte>(rhs.Data.ToArray());

        public static implicit operator ByteSpan(byte[] rhs) => new ByteSpan(rhs);
        public static implicit operator byte[](ByteSpan rhs) => rhs.Data.ToArray();
    }
}

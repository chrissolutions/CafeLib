using System;

namespace CafeLib.Bitcoin.Buffers
{
    public readonly ref struct ByteMemory
    {
        public Memory<byte> Data { get; }

        public ByteMemory(byte[] data = null)
        {
            Data = new Memory<byte>(data ?? Array.Empty<byte>());
        }

        public ByteMemory(ByteSpan data)
        {
            Data = new Memory<byte>(data);
        }

        public ByteMemory(Memory<byte> data)
        {
            Data = data;
        }

        public byte this[int index]
        {
            get => Data.Span[index];
            set => Data.Span[index] = value;
        }

        public ByteSpan this[Range range] => Data.Span[range];

        public void Reverse() => Data.Span.Reverse();

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ByteSpan Slice(int start) => Data.Span[start..];
        public ByteSpan Slice(int start, int length) => Data.Span.Slice(start, length);

        public void CopyTo(ByteSpan destination) => Data.Span.CopyTo(destination);
        public Span<byte>.Enumerator GetEnumerator() => Data.Span.GetEnumerator();
        public byte[] ToArray() => Data.ToArray();

        public static implicit operator Memory<byte>(ByteMemory rhs) => rhs.Data;
        public static implicit operator ByteMemory(Memory<byte> rhs) => new ByteMemory(rhs);

        public static implicit operator ByteSpan(ByteMemory rhs) => rhs.Data.Span;
        public static implicit operator ByteMemory(ByteSpan rhs) => new ByteMemory(rhs);

        public static implicit operator byte[](ByteMemory rhs) => rhs.Data.ToArray();
        public static implicit operator ByteMemory(byte[] rhs) => new ByteSpan(rhs);

        //public static ByteMemory operator +(ByteMemory a, ByteMemory b) => a + new UInt256(b);
    }
}

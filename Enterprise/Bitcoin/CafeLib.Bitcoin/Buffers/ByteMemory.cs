using System;

namespace CafeLib.Bitcoin.Buffers
{
    public readonly ref struct ByteMemory
    {
        internal Memory<byte> Data { get; }

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

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ByteMemory Slice(int start) => Data.Slice(start);
        public ByteMemory Slice(int start, int length) => Data.Slice(start, length);

        public void CopyTo(ByteMemory destination) => Data.CopyTo(destination);
        public Span<byte>.Enumerator GetEnumerator() => Data.Span.GetEnumerator();
        public byte[] ToArray() => Data.ToArray();
        public override string ToString() => Data.ToString();

        public static implicit operator Memory<byte>(ByteMemory rhs) => rhs.Data;
        public static implicit operator ByteMemory(Memory<byte> rhs) => new ByteMemory(rhs);

        public static implicit operator ByteSpan(ByteMemory rhs) => rhs.Data.Span;
        public static implicit operator ByteMemory(ByteSpan rhs) => new ByteMemory(rhs);

        public static implicit operator byte[](ByteMemory rhs) => rhs.Data.ToArray();
        public static implicit operator ByteMemory(byte[] rhs) => new ByteSpan(rhs);

        //public static ByteMemory operator +(ByteMemory a, ByteMemory b) => a + new UInt256(b);
    }
}

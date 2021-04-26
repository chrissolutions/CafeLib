using System;

namespace CafeLib.Bitcoin.Buffers
{
    public readonly ref struct ReadOnlyByteMemory
    {
        internal ReadOnlyMemory<byte> Data { get; }

        public ReadOnlyByteMemory(byte[] data = null)
        {
            Data = new ReadOnlyMemory<byte>(data ?? Array.Empty<byte>());
        }
        public ReadOnlyByteMemory(byte[] data, int start, int length)
        {
            Data = new ReadOnlyMemory<byte>(data, start, length);
        }

        public ReadOnlyByteMemory(ReadOnlyMemory<byte> data)
        {
            Data = data;
        }

        public ReadOnlyByteMemory(Memory<byte> data)
        {
            Data = data;
        }

        public ReadOnlyByteMemory(ByteSpan data)
        {
            Data = new ReadOnlyMemory<byte>(data);
        }

        public ReadOnlyByteMemory(ByteMemory data)
        {
            Data = data.Data;
        }

        public byte this[int index] => Data.Span[index];

        public ReadOnlyByteSpan this[Range range] => Data.Span[range];

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ReadOnlyByteSpan Slice(int start) => Data.Span[start..];
        public ReadOnlyByteSpan Slice(int start, int length) => Data.Span.Slice(start, length);

        public void CopyTo(ByteMemory destination) => Data.CopyTo(destination);
        public byte[] ToArray() => Data.ToArray();

        public static implicit operator ReadOnlyMemory<byte>(ReadOnlyByteMemory rhs) => rhs.Data;
        public static implicit operator ReadOnlyByteMemory(ReadOnlyMemory<byte> rhs) => new ReadOnlyByteMemory(rhs);
        public static implicit operator ReadOnlyByteMemory(Memory<byte> rhs) => new ReadOnlyByteMemory(rhs);
        public static implicit operator ReadOnlyByteMemory(ByteMemory rhs) => new ReadOnlyByteMemory(rhs);

        //public static ByteMemory operator +(ByteMemory a, ByteMemory b) => a + new UInt256(b);
    }
}

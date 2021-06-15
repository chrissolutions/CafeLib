using System;

namespace CafeLib.BsvSharp.Buffers
{
    public ref struct UInt64Span
    {
        public Span<ulong> Data { get; }

        public UInt64Span(Span<ulong> bytes)
        {
            Data = bytes;
        }

        public ulong this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public static implicit operator Span<ulong>(UInt64Span rhs)
        {
            return rhs.Data;
        }

        public static implicit operator UInt64Span(Span<ulong> rhs)
        {
            return new UInt64Span(rhs);
        }
    }
}

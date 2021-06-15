using System;

namespace CafeLib.BsvSharp.Buffers
{
    public ref struct UInt32Span
    {
        public Span<uint> Data { get; }

        public UInt32Span(Span<uint> data)
        {
            Data = data;
        }

        public uint this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public static implicit operator Span<uint>(UInt32Span rhs)
        {
            return rhs.Data;
        }

        public static implicit operator UInt32Span(Span<uint> rhs)
        {
            return new UInt32Span(rhs);
        }
    }
}

using System;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public ref struct Bytes
    {
        public Span<byte> Data { get; }

        public Bytes(Span<byte> bytes)
        {
            Data = bytes;
        }

        public static implicit operator Span<byte>(Bytes rhs)
        {
            return rhs.Data;
        }

        public static implicit operator Bytes(Span<byte> rhs)
        {
            return new Bytes(rhs);
        }
    }
}

using System;
using System.Runtime.CompilerServices;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public ref struct ByteSpan
    {
        public Span<byte> Data { get; }

        public ByteSpan(Span<byte> data)
        {
            Data = data;
        }

        public byte this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public void Reverse()
        {
            Data.Reverse();
        }

        public static implicit operator Span<byte>(ByteSpan rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ReadOnlySpan<byte>(ByteSpan rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ReadOnlyByteSpan(ByteSpan rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ByteSpan(Span<byte> rhs)
        {
            return new ByteSpan(rhs);
        }
    }
}

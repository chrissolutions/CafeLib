using System;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public ref struct ReadOnlyBytes
    {
        public ReadOnlySpan<byte> Data { get; }

        public ReadOnlyBytes(ReadOnlySpan<byte> bytes)
        {
            Data = bytes;
        }

        public static implicit operator ReadOnlySpan<byte>(ReadOnlyBytes rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ReadOnlyBytes(ReadOnlySpan<byte> rhs)
        {
            return new ReadOnlyBytes(rhs);
        }
    }
}

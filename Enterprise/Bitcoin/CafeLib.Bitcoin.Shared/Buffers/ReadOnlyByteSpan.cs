using System;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public readonly ref struct ReadOnlyByteSpan
    {
        public ReadOnlySpan<byte> Data { get; }
        public int Length => Data.Length;

        public ReadOnlyByteSpan(ReadOnlySpan<byte> data)
        {
            Data = data;
        }

        public static implicit operator ReadOnlySpan<byte>(ReadOnlyByteSpan rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ReadOnlyByteSpan(ReadOnlySpan<byte> rhs)
        {
            return new ReadOnlyByteSpan(rhs);
        }

        public static implicit operator ReadOnlyByteSpan(Span<byte> rhs)
        {
            return new ReadOnlyByteSpan(rhs);
        }

        public static implicit operator Span<byte>(ReadOnlyByteSpan rhs)
        {
            return new Span<byte>(rhs.Data.ToArray());
        }

        public static implicit operator byte[](ReadOnlyByteSpan rhs)
        {
            return rhs.Data.ToArray();
        }

        public static implicit operator ReadOnlyByteSpan(byte[] rhs)
        {
            return new ReadOnlyByteSpan(rhs);
        }
    }
}

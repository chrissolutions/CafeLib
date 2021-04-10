using System;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public ref struct ReadOnlyByteSpan
    {
        public ReadOnlySpan<byte> Data { get; }

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
    }
}

using System;
using System.Buffers;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public readonly ref struct ReadOnlyByteSpan
    {
        public ReadOnlySpan<byte> Data { get; }

        public ReadOnlyByteSpan(ReadOnlySpan<byte> data)
        {
            Data = data;
        }

        public ReadOnlyByteSpan(byte[] data)
        {
            Data = data;
        }

        public bool IsEmpty => Data.IsEmpty;
        public int Length => Data.Length;

        public ReadOnlyByteSpan Slice(int start) => Data[start..];
        public ReadOnlyByteSpan Slice(int start, int length) => Data.Slice(start, length);

        public void CopyTo(ByteSpan destination) => Data.CopyTo(destination);
        public ReadOnlySpan<byte>.Enumerator GetEnumerator() => Data.GetEnumerator();
        public byte[] ToArray() => Data.ToArray();

        public byte this[int index] => Data[index];
        public ReadOnlyByteSpan this[Range range] => Data[range];

        public static implicit operator ByteSpan(ReadOnlyByteSpan rhs) => rhs.Data;

        public static implicit operator ReadOnlySpan<byte>(ReadOnlyByteSpan rhs) => rhs.Data;
        public static implicit operator ReadOnlyByteSpan(ReadOnlySpan<byte> rhs) => new ReadOnlyByteSpan(rhs);

        public static implicit operator Span<byte>(ReadOnlyByteSpan rhs) => new Span<byte>(rhs.Data.ToArray());
        public static implicit operator ReadOnlyByteSpan(Span<byte> rhs) => new ReadOnlyByteSpan(rhs);

        public static implicit operator byte[](ReadOnlyByteSpan rhs) => rhs.Data.ToArray();
        public static implicit operator ReadOnlyByteSpan(byte[] rhs) => new ReadOnlyByteSpan(rhs);

        public static implicit operator ReadOnlyByteSpan(ReadOnlyByteSequence rhs)
        {
            return rhs.Data.IsSingleSegment
                ? new SequenceReader<byte>(rhs.Data).UnreadSpan
                : rhs.Data.ToArray();
        }
    }
}

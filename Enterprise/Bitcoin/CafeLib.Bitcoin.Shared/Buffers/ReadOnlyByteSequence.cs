using System.Buffers;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public readonly struct ReadOnlyByteSequence
    {
        public ReadOnlySequence<byte> Data { get; }

        public ReadOnlyByteSequence(byte[] data)
        {
            Data = new ReadOnlySequence<byte>(data);
        }

        public ReadOnlyByteSequence(ReadOnlySequence<byte> data)
        {
            Data = data;
        }

        public ReadOnlySequence<byte>.Enumerator GetEnumerator() => Data.GetEnumerator();

        public long Length => Data.Length;

        public void CopyTo(ByteSpan destination) => Data.CopyTo(destination);
        public byte[] ToArray() => Data.ToArray();

        public static implicit operator ReadOnlySequence<byte>(ReadOnlyByteSequence rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ReadOnlyByteSequence(ReadOnlySequence<byte> rhs)
        {
            return new ReadOnlyByteSequence(rhs);
        }
    }
}

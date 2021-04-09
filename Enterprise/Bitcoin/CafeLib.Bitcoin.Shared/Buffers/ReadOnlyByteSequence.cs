using System.Buffers;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public ref struct ReadOnlyByteSequence
    {
        public ReadOnlySequence<byte> Data { get; }

        public ReadOnlyByteSequence(ReadOnlySequence<byte> bytes)
        {
            Data = bytes;
        }

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

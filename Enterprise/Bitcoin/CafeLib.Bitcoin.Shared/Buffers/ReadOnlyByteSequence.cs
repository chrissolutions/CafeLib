using System;
using System.Buffers;
using System.Data;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public readonly ref struct ReadOnlyByteSequence
    {
        public ReadOnlySequence<byte> Data { get; }

        public ReadOnlyByteSequence(ReadOnlySequence<byte> bytes)
        {
            Data = bytes;
        }

        public ReadOnlySequence<byte>.Enumerator GetEnumerator() => Data.GetEnumerator();

        public long Length => Data.Length;

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

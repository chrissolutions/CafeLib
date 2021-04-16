using System.Buffers;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public ref struct ByteSequenceReader
    {
        internal SequenceReader<byte> Data;

        public ByteSequenceReader(SequenceReader<byte> sequence)
        {
            Data = sequence;
        }

        public ByteSequenceReader(ReadOnlyByteSequence sequence)
        {
            Data = new SequenceReader<byte>(sequence);
        }

        public static implicit operator SequenceReader<byte>(ByteSequenceReader rhs) => rhs.Data;
        public static implicit operator ByteSequenceReader(SequenceReader<byte> rhs) => new ByteSequenceReader(rhs);
    }
}

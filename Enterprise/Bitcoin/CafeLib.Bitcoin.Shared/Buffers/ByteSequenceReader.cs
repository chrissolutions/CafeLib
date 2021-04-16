using System;
using System.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;

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

        /// <summary>
        /// Reads an <see cref="long"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out int value) => Data.TryReadLittleEndian(out value);
        
        /// <summary>
        /// Reads an <see cref="long"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out uint value) => Data.TryReadLittleEndian(out value);

        /// <summary>
        /// Reads an <see cref="long"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out long value) => Data.TryReadLittleEndian(out value);

        /// <summary>
        /// Reads an <see cref="long"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out ulong value) => Data.TryReadLittleEndian(out value);

        public static implicit operator SequenceReader<byte>(ByteSequenceReader rhs) => rhs.Data;
        public static implicit operator ByteSequenceReader(SequenceReader<byte> rhs) => new ByteSequenceReader(rhs);
    }
}

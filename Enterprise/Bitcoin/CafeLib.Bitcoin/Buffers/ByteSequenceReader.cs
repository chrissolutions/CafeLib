using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Buffers
{
    public ref struct ByteSequenceReader
    {
        internal SequenceReader<byte> Data;

        public ByteSequenceReader(byte[] bytes)
        {
            Data = new SequenceReader<byte>(new ReadOnlySequence<byte>(bytes ?? Array.Empty<byte>()));
        }

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

        /// <summary>
        /// Reads an <see cref="byte"/>.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="byte"/>.</returns>
        public bool TryRead(out byte value) => Data.TryRead(out value);

        /// <summary>
        /// Reads an <see cref="short"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="short"/>.</returns>
        public bool TryReadBigEndian(out short value) => Data.TryReadBigEndian(out value);

        /// <summary>
        /// Reads an <see cref="ushort"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="ushort"/>.</returns>
        public bool TryReadBigEndian(out ushort value)
        {
            var result = Data.TryReadBigEndian(out short v);
            value = (ushort)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="int"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="int"/>.</returns>
        public bool TryReadBigEndian(out int value) => Data.TryReadBigEndian(out value);

        /// <summary>
        /// Reads an <see cref="uint"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="uint"/>.</returns>
        public bool TryReadBigEndian(out uint value)
        {
            var result = Data.TryReadBigEndian(out long v);
            value = (uint)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="long"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadBigEndian(out long value) => Data.TryReadBigEndian(out value);

        /// <summary>
        /// Reads an <see cref="ulong"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="ulong"/>.</returns>
        public bool TryReadBigEndian(out ulong value)
        {
            var result = Data.TryReadBigEndian(out long v);
            value = (ulong)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="long"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out short value) => Data.TryReadLittleEndian(out value);

        /// <summary>
        /// Reads an <see cref="long"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out ushort value)
        {
            var result = Data.TryReadLittleEndian(out short v);
            value = (ushort)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="int"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="int"/>.</returns>
        public bool TryReadLittleEndian(out int value) => Data.TryReadLittleEndian(out value);

        /// <summary>
        /// Reads an <see cref="uint"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="uint"/>.</returns>
        public bool TryReadLittleEndian(out uint value)
        {
            var result = Data.TryReadLittleEndian(out int v);
            value = (uint)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="long"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryReadLittleEndian(out long value) => Data.TryReadLittleEndian(out value);

        /// <summary>
        /// Reads an <see cref="ulong"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="ulong"/>.</returns>
        public bool TryReadLittleEndian(out ulong value)
        {
            var result = Data.TryReadLittleEndian(out long v);
            value = (ulong)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="UInt64"/> as in bitcoin VarInt format.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="UInt64"/>.</returns>
        public bool TryReadVarInt(out long value) => VarInt.TryRead(ref this, out value);

        /// <summary>
        /// Reads an <see cref="UInt256"/> as in bitcoin VarInt format.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadUInt256(ref UInt256 destination)
        {
            var bytes = destination.Span;
            if (!Data.TryCopyTo(bytes)) return false;
            Data.Advance(bytes.Length);
            return true;
        }
    }
}

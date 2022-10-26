using System;
using System.Buffers;

namespace CafeLib.Core.Buffers
{
    public ref struct ByteSequenceReader
    {
        public SequenceReader<byte> Data;

        public ByteSequenceReader(byte[] bytes)
        {
            Data = new SequenceReader<byte>(new ReadOnlyByteSequence(bytes ?? Array.Empty<byte>()));
        }

        public ByteSequenceReader(SequenceReader<byte> sequence)
        {
            Data = sequence;
        }

        public ByteSequenceReader(ReadOnlyByteSequence sequence)
        {
            Data = new SequenceReader<byte>(sequence);
        }

        public long Consumed => Data.Consumed;

        public ReadOnlyByteSpan CurrentSpan => Data.CurrentSpan;

        public int CurrentSpanIndex => Data.CurrentSpanIndex;

        public bool End => Data.End;

        public long Length => Data.Length;

        public SequencePosition Position => Data.Position;

        public long Remaining => Data.Remaining;

        public ReadOnlyByteSequence Sequence => Data.Sequence;

        public ReadOnlyByteSpan UnreadSpan => Data.UnreadSpan;

        public void Advance(long count) => Data.Advance(count);

        public long AdvancePast(byte value) => Data.AdvancePast(value);

        public long AdvancePastAny(ReadOnlyByteSpan values) => Data.AdvancePastAny(values);

        public long AdvancePastAny(byte value0, byte value1) => Data.AdvancePastAny(value0, value1);

        public long AdvancePastAny(byte value0, byte value1, byte value2) => Data.AdvancePastAny(value0, value1, value2);

        public long AdvancePastAny(byte value0, byte value1, byte value2, byte value3) => Data.AdvancePastAny(value0, value1, value2, value3);

        public bool IsNext(ReadOnlyByteSpan next, bool advancePast = false) => Data.IsNext(next, advancePast);

        public bool IsNext(byte next, bool advancePast = false) => Data.IsNext(next, advancePast);

        public void Rewind(long count) => Data.Rewind(count);

        public bool TryAdvanceTo(byte delimiter, bool advancePastDelimiter = true) => Data.TryAdvanceTo(delimiter, advancePastDelimiter);

        public bool TryAdvanceToAny(ReadOnlyByteSpan delimiters, bool advancePastDelimiter = true) => Data.TryAdvanceToAny(delimiters, advancePastDelimiter);

        public bool TryCopyTo(ByteSpan destination) => Data.TryCopyTo(destination);

        public bool TryPeek(out byte value) => Data.TryPeek(out value);

        public static implicit operator SequenceReader<byte>(ByteSequenceReader rhs) => rhs.Data;
        public static implicit operator ByteSequenceReader(SequenceReader<byte> rhs) => new ByteSequenceReader(rhs);

        /// <summary>
        /// Reads an <see cref="byte"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns></returns>
        public bool TryRead(out byte value) => Data.TryRead(out value);

        /// <summary>
        /// Reads an <see cref="short"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns>False if there wasn't enough data for an <see cref="short"/>.</returns>
        public bool TryRead(out short value) => BitConverter.IsLittleEndian ? Data.TryReadLittleEndian(out value) : Data.TryReadBigEndian(out value);

        /// <summary>
        /// Reads an <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns>False if there wasn't enough data for an <see cref="ushort"/>.</returns>
        public bool TryRead(out ushort value)
        {
            var result = TryRead(out short v);
            value = (ushort)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="int"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns>False if there wasn't enough data for an <see cref="ushort"/>.</returns>
        public bool TryRead(out int value) => BitConverter.IsLittleEndian ? Data.TryReadLittleEndian(out value) : Data.TryReadBigEndian(out value);

        /// <summary>
        /// Reads an <see cref="uint"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns>False if there wasn't enough data for an <see cref="uint"/>.</returns>
        public bool TryRead(out uint value)
        {
            var result = TryRead(out int v);
            value = (uint)v;
            return result;
        }

        /// <summary>
        /// Reads an <see cref="long"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public bool TryRead(out long value) => BitConverter.IsLittleEndian ? Data.TryReadLittleEndian(out value) : Data.TryReadBigEndian(out value);

        /// <summary>
        /// Reads an <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">out value</param>
        /// <returns>False if there wasn't enough data for an <see cref="ulong"/>.</returns>
        public bool TryRead(out ulong value)
        {
            var result = TryRead(out long v);
            value = (ulong)v;
            return result;
        }

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
            var result = Data.TryReadBigEndian(out int v);
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
    }
}

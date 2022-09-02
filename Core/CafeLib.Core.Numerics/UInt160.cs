using System;
using CafeLib.Core.Buffers;

namespace CafeLib.Core.Numerics
{
    public struct UInt160 : IComparable<UInt160>, IEquatable<UInt160>
    {
        private ulong _n0 = 0;
        private ulong _n1 = 0;
        private uint _n2 = 0;

        public const int Length = 2 * sizeof(ulong) + sizeof(uint);

        public static UInt160 Zero { get; } = new(0);
        public static UInt160 One { get; } = new(1);

        public UInt160(ReadOnlyByteSpan span, bool reverse = false)
        {
            if (span.Length < Length)
                throw new ArgumentException($"{Length} bytes are required.");

            span[..Length].CopyTo(Span);

            if (reverse)
            {
                Span.Reverse();
            }
        }

        public UInt160(uint v0, uint v1, uint v2, uint v3, uint v4)
            : this()
        {
            _n0 = v0 + ((ulong)v1 << 32);
            _n1 = v2 + ((ulong)v3 << 32);
            _n2 = v4;
        }

        public UInt160(ulong v0 = 0, ulong v1 = 0, uint v2 = 0)
            : this()
        {
            _n0 = v0;
            _n1 = v1;
            _n2 = v2;
        }

        public byte this[Index index]
        {
            get => Span[index];
            set => Span.Data[index] = value;
        }

        public UInt160 this[Range range] => new(Span[range]);

        /// <summary>
        /// Creates a UInt160 object from hex string.
        /// </summary>
        /// <param name="hex">hex string</param>
        /// <param name="bigEndian">assign bytes from highest to lowest numeric position</param>
        /// <returns>UInt160 object</returns>
        /// <remarks>Default behavior assigns bytes using little endian ordering</remarks>
        public static UInt160 FromHex(string hex, bool bigEndian = false)
        {
            if (string.IsNullOrWhiteSpace(hex)) return new UInt160();
            var result = new UInt160();
            (bigEndian ? Encoders.Hex : Encoders.HexReverse).TryDecodeSpan(hex, result.Span);
            return result;
        }

        /// <summary>
        /// ByteSpan property.
        /// </summary>
        public ByteSpan Span
        {
            get
            {
                unsafe
                {
                    fixed (ulong* p = &_n0)
                    {
                        var pb = (byte*)p;
                        var bytes = new Span<byte>(pb, Length);
                        return bytes;
                    }
                }
            }
        }

        /// <summary>
        /// Get the byte array 
        /// </summary>
        /// <param name="reverse"></param>
        /// <returns>byte array of the UInt160 object</returns>
        public byte[] ToArray(bool reverse = false) => !reverse ? Span : new ByteSpan(Span).Reverse();

        /// <summary>
        /// Copy byte array to destination
        /// </summary>
        /// <param name="destination">ByteSpan destination</param>
        /// <param name="reverse">reverse byte flag</param>
        /// <exception cref="ArgumentException">destination length is less than the UInt160 length</exception>
        public void CopyTo(ByteSpan destination, bool reverse = false)
        {
            if (destination.Length < Length)
                throw new ArgumentException($"{Length} byte destination is required.");
            Span.CopyTo(destination);
            if (reverse)
                destination.Reverse();
        }

        /// <summary>
        /// Convert numeric into hexadecimal string format.
        /// </summary>
        /// <param name="bigEndian">big endian order flag</param>
        /// <returns>hexadecimal string</returns>
		public string ToHex(bool bigEndian = false) => (bigEndian ? Encoders.Hex : Encoders.HexReverse)?.Encode(Span);

        /// <summary>
        /// The bytes appear based on the endian order specified during creation.
        /// The default behavior is for bytes to appear in big-endian order, as a large hexadecimal encoded number.
        ///
        /// When littleEndian is specified The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns>encoded string</returns>
        public override string ToString() => ToHex(!BitConverter.IsLittleEndian);

        public override int GetHashCode() => _n0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode();

        public override bool Equals(object obj) => obj is UInt160 int160 && this == int160;
        public bool Equals(UInt160 o) => _n0 == o._n0 && _n1 == o._n1 && _n2 == o._n2;

        public static bool operator ==(UInt160 x, UInt160 y) => x.Equals(y);
        public static bool operator !=(UInt160 x, UInt160 y) => !(x == y);

        public static explicit operator UInt160(byte[] rhs) => new(rhs);
        public static implicit operator byte[](UInt160 rhs) => rhs.Span.ToArray();

        public static explicit operator UInt160(ByteSpan rhs) => new(rhs);
        public static explicit operator ByteSpan(UInt160 rhs) => rhs.Span;

        public static explicit operator UInt160(ReadOnlyByteSpan rhs) => new(rhs);
        public static explicit operator ReadOnlyByteSpan(UInt160 rhs) => rhs.Span;

        public int CompareTo(UInt160 o)
        {
            var r = _n2.CompareTo(o._n2);
            if (r == 0) r = _n1.CompareTo(o._n1);
            if (r == 0) r = _n0.CompareTo(o._n0);
            return r;
        }

        public static UInt160 operator ~(UInt160 v)
        {
            v._n0 = ~v._n0;
            v._n1 = ~v._n1;
            v._n2 = ~v._n2;
            return v;
        }
    }
}

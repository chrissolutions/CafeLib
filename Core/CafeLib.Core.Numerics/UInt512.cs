using System;
using CafeLib.Core.Buffers;

namespace CafeLib.Core.Numerics
{
    public struct UInt512 : IComparable<UInt512>, IEquatable<UInt512>
    {
        private ulong _n0;
        private ulong _n1;
        private ulong _n2;
        private ulong _n3;
        private ulong _n4;
        private ulong _n5;
        private ulong _n6;
        private ulong _n7;

        private readonly bool _littleEndian;

        public const int Length = 8*sizeof(ulong);

        public static UInt512 Zero { get; } = new(0);
        public static UInt512 One { get; } = new(1);

        public UInt512(bool littleEndian = false)
        {
            _n0 = _n1 = _n2 = _n3 = _n4 = _n5 = _n6 = _n7 = 0;
            _littleEndian = littleEndian;
        }

        public UInt512(ReadOnlyByteSpan span, bool littleEndian = false)
            : this(littleEndian)
        {
            if (span.Length < Length)
                throw new ArgumentException($"{Length} bytes are required.");

            span[..Length].CopyTo(Span);

            if (littleEndian)
            {
                Span.Reverse();
            }
        }

        public UInt512(ulong v0 = 0, ulong v1 = 0, ulong v2 = 0, ulong v3 = 0, ulong v4 = 0, ulong v5 = 0, ulong v6 = 0, ulong v7 = 0)
            : this()
        {
            _n0 = v0;
            _n1 = v1;
            _n2 = v2;
            _n3 = v3;
            _n4 = v4;
            _n5 = v5;
            _n6 = v6;
            _n7 = v7;
        }

        public byte this[Index index]
        {
            get => Span[index];
            set => Span.Data[index] = value;
        }

        public UInt256 this[Range range] => new(Span[range]);

        /// <summary>
        /// Creates a UInt512 object from hex string.
        /// </summary>
        /// <param name="hex">hex string</param>
        /// <param name="littleEndian">assign bytes from lowest to highest numeric position</param>
        /// <returns>UInt256 object</returns>
        /// <remarks>Default behavior assigns bytes from highest to lowest numeric position</remarks>
        public static UInt512 FromHex(string hex, bool littleEndian = false)
        {
            if (string.IsNullOrWhiteSpace(hex)) return new UInt512();
            var result = new UInt512(littleEndian);
            (littleEndian ? Encoders.Hex : Encoders.HexReverse).TryDecodeSpan(hex, result.Span);
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
        /// <returns>byte array of the UInt512 object</returns>
        public byte[] ToArray(bool reverse = false) => !reverse ? Span : new ByteSpan(Span).Reverse();

        /// <summary>
        /// Copy byte array to destination
        /// </summary>
        /// <param name="destination">ByteSpan destination</param>
        /// <param name="reverse">reverse byte flag</param>
        /// <exception cref="ArgumentException">destination length is less than the UInt512 length</exception>
        public void CopyTo(ByteSpan destination, bool reverse = false)
        {
            if (destination.Length < Length)
                throw new ArgumentException($"{Length} byte destination is required.");
            Span.CopyTo(destination);
            if (reverse)
                destination.Reverse();
        }

        /// <summary>
        /// The bytes appear based on the endian order specified during creation.
        /// The default behavior is for bytes to appear in big-endian order, as a large hexadecimal encoded number.
        ///
        /// When littleEndian is specified The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns>encoded string</returns>
        public override string ToString() => (_littleEndian ? Encoders.Hex : Encoders.HexReverse)?.Encode(Span);

        public override int GetHashCode() => _n0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode() ^ _n3.GetHashCode();

        public override bool Equals(object obj) => obj is UInt512 int512 && this == int512;

        public bool Equals(UInt512 o) => _n0 == o._n0 && _n1 == o._n1 && _n2 == o._n2 && _n3 == o._n3 && _n4 == o._n4 && _n5 == o._n5 && _n6 == o._n6 && _n7 == o._n7;

        public static explicit operator UInt512(byte[] rhs) => new(rhs);
        public static implicit operator byte[](UInt512 rhs) => rhs.Span.ToArray();

        public static explicit operator UInt512(ByteSpan rhs) => new(rhs);
        public static explicit operator UInt512(ReadOnlyByteSpan rhs) => new(rhs);

        public static implicit operator ByteSpan(UInt512 rhs) => rhs.Span;
        public static implicit operator ReadOnlyByteSpan(UInt512 rhs) => rhs.Span;

        public static bool operator ==(UInt512 x, UInt512 y) => x.Equals(y);
        public static bool operator !=(UInt512 x, UInt512 y) => !(x == y);

        public int CompareTo(UInt512 o)
        {
            var r = _n7.CompareTo(o._n7);
            if (r == 0) r = _n6.CompareTo(o._n6);
            if (r == 0) r = _n5.CompareTo(o._n5);
            if (r == 0) r = _n4.CompareTo(o._n4);
            if (r == 0) r = _n3.CompareTo(o._n3);
            if (r == 0) r = _n2.CompareTo(o._n2);
            if (r == 0) r = _n1.CompareTo(o._n1);
            if (r == 0) r = _n0.CompareTo(o._n0);
            return r;
        }

        public static UInt512 operator ^(UInt512 x, UInt512 y)
        {
            var r = new UInt512
            {
                _n0 = x._n0 ^ y._n0,
                _n1 = x._n1 ^ y._n1,
                _n2 = x._n2 ^ y._n2,
                _n3 = x._n3 ^ y._n3,
                _n4 = x._n4 ^ y._n4,
                _n5 = x._n5 ^ y._n5,
                _n6 = x._n6 ^ y._n6,
                _n7 = x._n7 ^ y._n7
            };
            return r;
        }
    }
}

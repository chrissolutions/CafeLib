using System;
using CafeLib.Core.Buffers;

namespace CafeLib.Core.Numerics
{
    public struct UInt256 : IComparable<UInt256>, IEquatable<UInt256>
    {
        private ulong _n0 = 0;
        private ulong _n1 = 0;
        private ulong _n2 = 0;
        private ulong _n3 = 0;

        public const int Length = 4 * sizeof(ulong);

        public static UInt256 Zero { get; } = new(0);
        public static UInt256 One { get; } = new(1);

        public UInt256(UInt256 uint256)
            : this(uint256.Span)
        {
        }

        public UInt256(ReadOnlyByteSpan span, bool reverse = false)
        {
            if (span.Length < Length)
                throw new ArgumentException("32 bytes are required.");

            span[..Length].CopyTo(Span);

            if (reverse)
            {
                Span.Reverse();
            }
        }

        public UInt256(ulong v0, ulong v1, ulong v2, ulong v3)
            : this()
        {
            _n0 = v0;
            _n1 = v1;
            _n2 = v2;
            _n3 = v3;
        }

        public byte this[Index index]
        {
            get => Span[index];
            set => Span.Data[index] = value;
        }

        public UInt256 this[Range range] => new(Span[range]);

        /// <summary>
        /// Creates a UInt256 object from hex string.
        /// </summary>
        /// <param name="hex">hex string</param>
        /// <param name="bigEndian">assign bytes from highest to lowest numeric position</param>
        /// <returns>UInt160 object</returns>
        /// <remarks>Default behavior assigns bytes using little endian ordering</remarks>
        public static UInt256 FromHex(string hex, bool bigEndian = false)
        {
            if (string.IsNullOrWhiteSpace(hex)) return new UInt256();
            var result = new UInt256();
            (bigEndian ? Encoders.Hex : Encoders.HexReverse).TryDecodeSpan(hex, result.Span);
            return result;
        }

        /// <summary>
        /// ByteSpan property.
        /// </summary>
        public readonly ByteSpan Span
        {
            get
            {
                unsafe
                {
                    fixed (ulong* p = &_n0)
                    {
                        var pb = (byte*)p;
                        var span = new Span<byte>(pb, Length);
                        return span;
                    }
                }
            }
        }

        /// <summary>
        /// Get the byte array 
        /// </summary>
        /// <param name="reverse"></param>
        /// <returns>byte array of the UInt256 object</returns>
        public byte[] ToArray(bool reverse = false) => !reverse ? Span : new ByteSpan(Span).Reverse();

        /// <summary>
        /// Copy byte array to destination
        /// </summary>
        /// <param name="destination">ByteSpan destination</param>
        /// <param name="reverse">reverse byte flag</param>
        /// <exception cref="ArgumentException">destination length is less than the UInt256 length</exception>
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

        public override int GetHashCode() => _n0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode() ^ _n3.GetHashCode();

        public override bool Equals(object obj) => obj is UInt256 int256 && this == int256;
        public readonly bool Equals(UInt256 o) => _n0 == o._n0 && _n1 == o._n1 && _n2 == o._n2 && _n3 == o._n3;

        public static bool operator ==(UInt256 x, UInt256 y) => x.Equals(y);
        public static bool operator !=(UInt256 x, UInt256 y) => !(x == y);

        public static bool operator >(UInt256 x, UInt256 y) => x.CompareTo(y) > 0;
        public static bool operator <(UInt256 x, UInt256 y) => x.CompareTo(y) < 0;

        public static bool operator >=(UInt256 x, UInt256 y) => x.CompareTo(y) >= 0;
        public static bool operator <=(UInt256 x, UInt256 y) => x.CompareTo(y) <= 0;

        public static explicit operator UInt256(byte[] rhs) => new(rhs);
        public static implicit operator byte[](UInt256 rhs) => rhs.Span.ToArray();

        public static implicit operator int(UInt256 rhs) => (int)rhs._n0;
        public static implicit operator UInt256(int rhs) => new((ulong)rhs, 0, 0, 0);
        public static implicit operator uint(UInt256 rhs) => (uint)rhs._n0;
        public static implicit operator UInt256(uint rhs) => new(rhs, 0, 0, 0);
        public static implicit operator long(UInt256 rhs) => (long)rhs._n0;
        public static implicit operator UInt256(long rhs) => new((ulong)rhs, 0, 0, 0);
        public static implicit operator ulong(UInt256 rhs) => rhs._n0;
        public static implicit operator UInt256(ulong rhs) => new(rhs, 0, 0, 0);

        public static explicit operator UInt256(ByteSpan rhs) => new(rhs);
        public static explicit operator ByteSpan(UInt256 rhs) => rhs.Span;

        public static explicit operator UInt256(ReadOnlyByteSpan rhs) => new(rhs);
        public static explicit operator ReadOnlyByteSpan(UInt256 rhs) => rhs.Span;

        public int CompareTo(UInt256 o)
        {
            var r = _n3.CompareTo(o._n3);
            if (r == 0) r = _n2.CompareTo(o._n2);
            if (r == 0) r = _n1.CompareTo(o._n1);
            if (r == 0) r = _n0.CompareTo(o._n0);
            return r;
        }

        public static UInt256 operator <<(UInt256 a, int shift)
        {
            const int width = 4;
            var r = Zero;
            var rpn = r.Span64.Data;
            var apn = a.Span64.Data;
            var k = shift / 64;
            shift %= 64;
            for (var i = 0; i < width; i++)
            {
                if (i + k + 1 < width && shift != 0)
                    rpn[i + k + 1] |= (apn[i] >> (64 - shift));

                if (i + k < width)
                    rpn[i + k] |= (apn[i] << shift);
            }
            return r;
        }

        public static UInt256 operator >>(UInt256 a, int shift)
        {
            const int width = 4;
            var r = Zero;
            var rpn = r.Span64.Data;
            var apn = a.Span64.Data;
            var k = shift / 64;
            shift %= 64;
            for (var i = 0; i < width; i++)
            {
                if (i - k - 1 >= 0 && shift != 0)
                    rpn[i - k - 1] |= (apn[i] << (64 - shift));

                if (i - k >= 0)
                    rpn[i - k] |= (apn[i] >> shift);
            }
            return r;
        }

        public static UInt256 operator ~(UInt256 v)
        {
            v._n0 = ~v._n0;
            v._n1 = ~v._n1;
            v._n2 = ~v._n2;
            v._n3 = ~v._n3;
            return v;
        }

        public int Bits()
        {
            const int width = Length / sizeof(uint);
            var pn = Span32;
            for (var pos = width - 1; pos >= 0; pos--)
            {
                if (pn[pos] == 0) continue;
                for (var bits = 31; bits > 0; bits--)
                {
                    if ((pn[pos] & (1 << bits)) != 0)
                    {
                        return 32 * pos + bits + 1;
                    }
                }

                return 32 * pos + 1;
            }

            return 0;
        }

        public static UInt256 operator ++(UInt256 a)
        {
            const int width = 4;
            var apn = a.Span64;
            int i = 0;
            while (++apn[i] == 0 && i < width - 1)
                i++;
            return a;
        }

        public static UInt256 operator -(UInt256 a, UInt256 b) => a + -b;
        public static UInt256 operator +(UInt256 a, ulong b) => a + new UInt256(b);
        public static UInt256 operator +(UInt256 a, UInt256 b)
        {
            const int width = 8;
            ulong carry = 0;
            var r = Zero;
            var rpn = r.Span32;
            var apn = a.Span32;
            var bpn = b.Span32;
            for (int i = 0; i < width; i++)
            {
                ulong n = carry + apn[i] + bpn[i];
                rpn[i] = (uint)(n & 0xffffffff);
                carry = n >> 32;
            }
            return r;
        }

        public static UInt256 operator -(UInt256 a)
        {
            const int width = 4;
            var r = Zero;
            var rpn = r.Span64;
            var apn = a.Span64;
            for (int i = 0; i < width; i++)
                rpn[i] = ~apn[i];
            r++;
            return r;
        }

        public static UInt256 operator |(UInt256 a, UInt256 b)
        {
            const int width = 4;
            var r = Zero;
            var rpn = r.Span64;
            var apn = a.Span64;
            var bpn = b.Span64;
            for (int i = 0; i < width; i++)
                rpn[i] = apn[i] | bpn[i];
            return r;
        }

        public static UInt256 operator /(UInt256 a, UInt256 b)
        {
            var num = a;
            var div = b;
            var r = Zero;

            int numBits = num.Bits();
            int divBits = div.Bits();
            if (divBits == 0) throw new ArgumentException("Division by zero");
            // the result is certainly 0.
            if (divBits > numBits)
                return r;

            var rpn = r.Span32;
            int shift = numBits - divBits;
            // shift so that div and num align.
            div <<= shift;
            while (shift >= 0)
            {
                if (num >= div)
                {
                    num -= div;
                    // set a bit of the result.
                    rpn[shift / 32] |= (uint)(1 << (shift & 31));
                }
                // shift back.
                div >>= 1;
                shift--;
            }
            // num now contains the remainder of the division.
            return r;
        }

        #region Helpers

        private UInt64Span Span64
        {
            get
            {
                unsafe
                {
                    fixed (ulong* p = &_n0)
                    {
                        var pb = p;
                        var span = new Span<ulong>(pb, Length / sizeof(ulong));
                        return span;
                    }
                }
            }
        }

        private UInt32Span Span32
        {
            get
            {
                unsafe
                {
                    fixed (ulong* p = &_n0)
                    {
                        var pb = (uint*)p;
                        var span = new Span<uint>(pb, Length / sizeof(uint));
                        return span;
                    }
                }
            }
        }

        #endregion
    }
}

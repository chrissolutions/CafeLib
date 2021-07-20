#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using System.Numerics;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Numerics.Converters;
using CafeLib.Core.Buffers;
using Newtonsoft.Json;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.BsvSharp.Numerics
{
    [JsonConverter(typeof(JsonConverterUInt256))]
    public struct UInt256 : IComparable<UInt256>
    {
        private ulong _n0;
        private ulong _n1;
        private ulong _n2;
        private ulong _n3;

        public const int Length = 32;

        public UInt256(UInt256 uint256)
            : this(uint256.Span)
        {
        }

        public UInt256(ReadOnlyByteSpan span, bool reverse = false)
            : this()
        {
            if (span.Length < Length)
                throw new ArgumentException("32 bytes are required.");

            span.Slice(0, 32).CopyTo(Span);
            if (reverse)
                Span.Reverse();
        }

		public UInt256(ulong v0 = 0, ulong v1 = 0, ulong v2 = 0, ulong v3 = 0)
		{
            _n0 = v0;
            _n1 = v1;
            _n2 = v2;
            _n3 = v3;
		}

        public UInt256(string hex, bool firstByteFirst = false)
            : this()
        {
            (firstByteFirst ? Encoders.Hex : Encoders.HexReverse).TryDecodeSpan(hex, Span);
        }

        public static UInt256 Zero { get; } = new UInt256(0);
        public static UInt256 One { get; } = new UInt256(1);

        public UInt64Span Span64
        {
            get
            {
                unsafe 
                {
                    fixed (ulong* p = &_n0)
                    {
                        var pb = p;
                        var span = new Span<ulong>(pb, Length/sizeof(ulong));
                        return span;
                    }
                }
            }
        }

        public UInt32Span Span32
        {
            get 
            {
                unsafe
                {
                    fixed (ulong* p = &_n0)
                    {
                        var pb = (uint*)p;
                        var span = new Span<uint>(pb, Length/sizeof(uint));
                        return span;
                    }
                }
            }
        }

        public readonly ByteSpan Span
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

        public void Read(BinaryReader s)
        {
            s.Read(Span);
        }

        public readonly BigInteger ToBigInteger() => new BigInteger(Span, isUnsigned:true, isBigEndian:true);

        public byte[] ToArray(bool reverse = false)  => !reverse ? Span : new ByteSpan(Span).Reverse();

        public void ToArray(ByteSpan destination, bool reverse = false)
        {
            if (destination.Length < Length)
                throw new ArgumentException($"{Length} byte destination is required.");
            Span.CopyTo(destination);
            if (reverse)
                destination.Reverse();
        }

        /// <summary>
        /// The bytes appear in big-endian order, as a large hexadecimal encoded number.
        /// </summary>
        /// <returns></returns>
		public override string ToString() => Encoders.HexReverse.Encode(Span);

        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// Equivalent to ToHex.
        /// </summary>
        /// <returns></returns>
		public readonly string ToStringFirstByteFirst() => Encoders.Hex.Encode(Span);

        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns></returns>
		public string ToHex() => Encoders.Hex.Encode(Span);

        public override int GetHashCode() => _n0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode() ^ _n3.GetHashCode();

        public override bool Equals(object obj) => obj is UInt256 int256 && this == int256;
        public readonly bool Equals(UInt256 o) => _n0 == o._n0 && _n1 == o._n1 && _n2 == o._n2 && _n3 == o._n3;

        public static bool operator ==(UInt256 x, UInt256 y) => x.Equals(y);
        public static bool operator !=(UInt256 x, UInt256 y) => !(x == y);

        public static bool operator >(UInt256 x, UInt256 y) => x.CompareTo(y) > 0;
        public static bool operator <(UInt256 x, UInt256 y) => x.CompareTo(y) < 0;

        public static bool operator >=(UInt256 x, UInt256 y) => x.CompareTo(y) >= 0;
        public static bool operator <=(UInt256 x, UInt256 y) => x.CompareTo(y) <= 0;

        public static explicit operator UInt256(byte[] rhs) => new UInt256(rhs);
        public static implicit operator byte[](UInt256 rhs) => rhs.Span.ToArray();

        public static implicit operator int(UInt256 rhs) => (int)rhs._n0;
        public static implicit operator UInt256(int rhs) => new UInt256((ulong)rhs);
        public static implicit operator uint(UInt256 rhs) => (uint)rhs._n0;
        public static implicit operator UInt256(uint rhs) => new UInt256((ulong)rhs);
        public static implicit operator long(UInt256 rhs) => (long)rhs._n0;
        public static implicit operator UInt256(long rhs) => new UInt256((ulong)rhs);
        public static implicit operator ulong(UInt256 rhs) => rhs._n0;
        public static implicit operator UInt256(ulong rhs) => new UInt256(rhs);

        public static explicit operator UInt256(ByteSpan rhs) => new UInt256(rhs);
        public static explicit operator UInt256(ReadOnlyByteSpan rhs) => new UInt256(rhs);

        public static implicit operator ByteSpan(UInt256 rhs) => rhs.Span;
        public static implicit operator ReadOnlyByteSpan(UInt256 rhs) => rhs.Span;

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
            var r = UInt256.Zero;
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
            var r = UInt256.Zero;
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

        public static UInt256 operator ~(UInt256 v) {
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
        public static UInt256 operator +(UInt256 a, UInt64 b) => a + new UInt256(b);
        public static UInt256 operator +(UInt256 a, UInt256 b) {
            const int width = 8;
            UInt64 carry = 0;
            var r = UInt256.Zero;
            var rpn = r.Span32;
            var apn = a.Span32;
            var bpn = b.Span32;
            for (int i = 0; i < width; i++) {
                UInt64 n = carry + apn[i] + bpn[i];
                rpn[i] = (UInt32)(n & 0xffffffff);
                carry = n >> 32;
            }
            return r;
        }

        public static UInt256 operator -(UInt256 a) {
            const int width = 4;
            var r = UInt256.Zero;
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
            var r = UInt256.Zero;
            var rpn = r.Span64;
            var apn = a.Span64;
            var bpn = b.Span64;
            for (int i = 0; i<width; i++)
                rpn[i] = apn[i] | bpn[i];
            return r;
        }

        public static UInt256 operator /(UInt256 a, UInt256 b) {
            var num = a;
            var div = b;
            var r = UInt256.Zero;

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
            while (shift >= 0) {
                if (num >= div) {
                    num -= div;
                    // set a bit of the result.
                    rpn[shift / 32] |= (UInt32)(1 << (shift & 31));
                }
                // shift back.
                div >>= 1;
                shift--;
            }
            // num now contains the remainder of the division.
            return r;
        }

#if false
        template <unsigned int BITS> unsigned int base_uint<BITS>::bits() const {
            for (int pos = WIDTH - 1; pos >= 0; pos--) {
                if (pn[pos]) {
                    for (int bits = 31; bits > 0; bits--) {
                        if (pn[pos] & 1 << bits) return 32 * pos + bits + 1;
                    }
                    return 32 * pos + 1;
                }
            }
            return 0;
        }

        base_uint<BITS> &base_uint<BITS>::operator<<=(unsigned int shift) {
            base_uint<BITS> a(*this);
            for (int i = 0; i < WIDTH; i++)
                pn[i] = 0;
            int k = shift / 32;
            shift = shift % 32;
            for (int i = 0; i < WIDTH; i++) {
                if (i + k + 1 < WIDTH && shift != 0)
                    pn[i + k + 1] |= (a.pn[i] >> (32 - shift));
                if (i + k < WIDTH) pn[i + k] |= (a.pn[i] << shift);
            }
            return *this;
        }

        template<unsigned int BITS>
        base_uint<BITS> &base_uint<BITS>::operator>>=(unsigned int shift) {
            base_uint<BITS> a(*this);
            for (int i = 0; i < WIDTH; i++)
                pn[i] = 0;
            int k = shift / 32;
            shift = shift % 32;
            for (int i = 0; i < WIDTH; i++) {
                if (i - k - 1 >= 0 && shift != 0)
                    pn[i - k - 1] |= (a.pn[i] << (32 - shift));
                if (i - k >= 0) pn[i - k] |= (a.pn[i] >> shift);
            }
            return *this;
        }
#endif
        }
}

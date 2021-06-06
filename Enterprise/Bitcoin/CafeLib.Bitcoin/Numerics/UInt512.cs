#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using System.Numerics;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Encoding;
using CafeLib.Bitcoin.Numerics.Converters;
using Newtonsoft.Json;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.Bitcoin.Numerics
{
    [JsonConverter(typeof(JsonConverterUInt512))]
    public struct UInt512
    {
        private ulong _n0;
        private ulong _n1;
        private ulong _n2;
        private ulong _n3;
        private ulong _n4;
        private ulong _n5;
        private ulong _n6;
        private ulong _n7;

        private static readonly Encoder Hex = Encoders.Hex;
        private static readonly Encoder HexReverse = Encoders.HexReverse;

        public const int Length = 64;

        public UInt512(ReadOnlyByteSpan span, bool reverse = false)
            : this()
        {
            if (span.Length < Length)
                throw new ArgumentException($"{Length} bytes are required.");

            span.Slice(0, Length).CopyTo(Span);
            if (reverse)
                Span.Reverse();
        }

        public UInt512(ulong v0 = 0, ulong v1 = 0, ulong v2 = 0, ulong v3 = 0, ulong v4 = 0, ulong v5 = 0, ulong v6 = 0, ulong v7 = 0)
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

        public UInt512(string hex, bool firstByteFirst = false)
            : this()
        {
            (firstByteFirst ? Hex : HexReverse).TryDecode(hex, Span);
        }

        public static UInt512 Zero { get; } = new UInt512(0);
        public static UInt512 One { get; } = new UInt512(1);

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

        public void Read(BinaryReader s)
        {
            s.Read(Span);
        }

        public BigInteger ToBigInteger() => new BigInteger(Span);

        /// <summary>
        /// The bytes appear in big-endian order, as a large hexadecimal encoded number.
        /// </summary>
        /// <returns></returns>
		public override string ToString() => HexReverse.Encode(Span);

        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns></returns>
		public string ToHex() => Hex.Encode(Span);

        public override int GetHashCode() => _n0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode() ^ _n3.GetHashCode();

        public override bool Equals(object obj) => obj is UInt512 int512 && this == int512;

        public bool Equals(UInt512 o) => _n0 == o._n0 && _n1 == o._n1 && _n2 == o._n2 && _n3 == o._n3 && _n4 == o._n4 && _n5 == o._n5 && _n6 == o._n6 && _n7 == o._n7;

        public static explicit operator UInt512(byte[] rhs) => new UInt512(rhs);
        public static implicit operator byte[](UInt512 rhs) => rhs.Span.ToArray();

        public static explicit operator UInt512(ByteSpan rhs) => new UInt512(rhs);
        public static explicit operator UInt512(ReadOnlyByteSpan rhs) => new UInt512(rhs);

        public static implicit operator ByteSpan(UInt512 rhs) => rhs.Span;
        public static implicit operator ReadOnlyByteSpan(UInt512 rhs) => rhs.Span;

        public static bool operator ==(UInt512 x, UInt512 y) => x.Equals(y);
        public static bool operator !=(UInt512 x, UInt512 y) => !(x == y);

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

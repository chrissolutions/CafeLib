#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using System.Numerics;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Numerics.Converters;
using CafeLib.BsvSharp.Services;
using Newtonsoft.Json;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.BsvSharp.Numerics
{
    [JsonConverter(typeof(JsonConverterUInt160))]
    public struct UInt160 : IComparable<UInt160>
    {
        private ulong _n0;
        private ulong _n1;
        private uint _n2;

        public const int Length = 20;

		public UInt160(ReadOnlyByteSpan span, bool reverse = false) : this()
        {
            if (span.Length < Length)
                throw new ArgumentException($"{Length} bytes are required.");
            span.Slice(0, Length).CopyTo(Span);
            if (reverse)
                Span.Reverse();
        }

		public UInt160(uint v0 = 0, uint v1 = 0, uint v2 = 0, uint v3 = 0, uint v4 = 0)
		{
            _n0 = v0 + ((ulong)v1 << 32);
            _n1 = v2 + ((ulong)v3 << 32);
            _n2 = v4;
		}

		public UInt160(ulong v0 = 0, ulong v1 = 0, uint v2 = 0)
		{
            _n0 = v0;
            _n1 = v1;
            _n2 = v2;
		}

        public UInt160(string hex, bool firstByteFirst = false) : this()
        {
            (firstByteFirst ? Encoders.Hex : Encoders.HexReverse).TryDecode(hex, Span);
        }

        public static UInt160 Zero { get; } = new UInt160(0);
        public static UInt160 One { get; } = new UInt160(1);

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

        public string ToPublicKeyAddress() => Encoders.Base58Check.Encode(RootService.Network.PublicKeyAddress, Span);
        public BigInteger ToBigInteger() => new BigInteger(Span, isUnsigned:true, isBigEndian:true);


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
		public string ToStringFirstByteFirst() => Encoders.Hex.Encode(Span);

        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns></returns>
		public string ToHex() => Encoders.Hex.Encode(Span);

        public override int GetHashCode() => _n0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode();

        public override bool Equals(object obj) => obj is UInt160 int160 && this == int160;
        public bool Equals(UInt160 o) => _n0 == o._n0 && _n1 == o._n1 && _n2 == o._n2;

        public static bool operator ==(UInt160 x, UInt160 y) => x.Equals(y);
        public static bool operator !=(UInt160 x, UInt160 y) => !(x == y);

        public static explicit operator UInt160(byte[] rhs) => new UInt160(rhs);
        public static implicit operator byte[](UInt160 rhs) => rhs.Span.ToArray();

        public static explicit operator UInt160(ByteSpan rhs) => new UInt160(rhs);
        public static implicit operator ByteSpan(UInt160 rhs) => rhs.Span;

        public static explicit operator UInt160(ReadOnlyByteSpan rhs) => new UInt160(rhs);
        public static implicit operator ReadOnlyByteSpan(UInt160 rhs) => rhs.Span;

        public int CompareTo(UInt160 o)
        {
            var r = _n2.CompareTo(o._n2);
            if (r == 0) r = _n1.CompareTo(o._n1);
            if (r == 0) r = _n0.CompareTo(o._n0);
            return r;
        }
    }
}

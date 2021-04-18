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
using CafeLib.Bitcoin.Services;
using Newtonsoft.Json;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.Bitcoin.Numerics
{
    [JsonConverter(typeof(JsonConverterUInt160))]
    public struct UInt160 : IComparable<UInt160>
    {
        public UInt64 N0;
        public UInt64 N1;
        public UInt32 N2;

        public const int Length = 20;

		public UInt160(ReadOnlyByteSpan span, bool reverse = false) : this()
        {
            if (span.Length < 20)
                throw new ArgumentException("20 bytes are required.");
            span.Slice(0, 20).CopyTo(Span);
            if (reverse)
                Span.Reverse();
        }

		public UInt160(UInt32 v0 = 0, UInt32 v1 = 0, UInt32 v2 = 0, UInt32 v3 = 0, UInt32 v4 = 0)
		{
            N0 = v0 + ((UInt64)v1 << 32);
            N1 = v2 + ((UInt64)v3 << 32);
            N2 = v4;
		}

		public UInt160(UInt64 v0 = 0, UInt64 v1 = 0, UInt32 v2 = 0)
		{
            N0 = v0;
            N1 = v1;
            N2 = v2;
		}

        public UInt160(string hex, bool firstByteFirst = false) : this()
        {
            (firstByteFirst ? Encoders.Hex : Encoders.HexReverse).TryDecode(hex, Span);
        }

        public static UInt160 Zero { get; } = new UInt160(0);
        public static UInt160 One { get; } = new UInt160(1);

        public ByteSpan Span {
            get {
                unsafe {
                    fixed (UInt64* p = &N0) {
                        byte* pb = (byte*)p;
                        var bytes = new Span<byte>(pb, 20);
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

        public byte[] ToBytes() => Span.ToArray();

        public void ToBytes(Span<byte> destination, bool reverse = false) {
            if (destination.Length < 20)
                throw new ArgumentException("20 byte destination is required.");
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

        public override int GetHashCode() => N0.GetHashCode() ^ N1.GetHashCode() ^ N2.GetHashCode();

        public override bool Equals(object obj) => obj is UInt160 int160 && this == int160;
        public bool Equals(UInt160 o) => N0 == o.N0 && N1 == o.N1 && N2 == o.N2;

        public static bool operator ==(UInt160 x, UInt160 y) => x.Equals(y);
        public static bool operator !=(UInt160 x, UInt160 y) => !(x == y);

        public static explicit operator UInt160(ByteSpan rhs) => new UInt160(rhs);
        public static explicit operator UInt160(ReadOnlyByteSpan rhs) => new UInt160(rhs);
        public static implicit operator ByteSpan(UInt160 rhs) => rhs.Span;
        public static implicit operator ReadOnlyByteSpan(UInt160 rhs) => rhs.Span;

        public int CompareTo(UInt160 o)
        {
            var r = N2.CompareTo(o.N2);
            if (r == 0) r = N1.CompareTo(o.N1);
            if (r == 0) r = N0.CompareTo(o.N0);
            return r;
        }
    }
}

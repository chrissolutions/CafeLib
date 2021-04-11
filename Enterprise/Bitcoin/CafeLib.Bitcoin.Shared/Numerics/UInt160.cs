#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using System.Numerics;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Numerics.Converters;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Shared.Numerics
{

    [JsonConverter(typeof(JsonConverterUInt160))]
    public struct UInt160 : IComparable<UInt160>
    {
        public UInt64 N0;
        public UInt64 N1;
        public UInt32 N2;

        public int Length => 20;

		public UInt160(ReadOnlySpan<byte> span, bool reverse = false) : this()
        {
            if (span.Length < 20)
                throw new ArgumentException("20 bytes are required.");
            span.Slice(0, 20).CopyTo(Bytes);
            if (reverse)
                Bytes.Reverse();
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
            (firstByteFirst ? Encoders.Hex : Encoders.HexReverse).TryDecode(hex, Bytes);
        }

        public static UInt160 Zero { get; } = new UInt160(0);
        public static UInt160 One { get; } = new UInt160(1);

        public ReadOnlyByteSpan ReadOnlyBytes => Bytes;

        public ByteSpan Bytes {
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
            s.Read(Bytes);
        }

        //public string ToPubKeyAddress() => KzEncoders.B58Check.Encode(Kz.PubkeyAddress, ReadOnlySpan);
        public BigInteger ToBigInteger() => new BigInteger(ReadOnlyBytes, isUnsigned:true, isBigEndian:true);

        public byte[] ToBytes() => Bytes.Data.ToArray();

        public void ToBytes(Span<byte> destination, bool reverse = false) {
            if (destination.Length < 20)
                throw new ArgumentException("20 byte destination is required.");
            Bytes.Data.CopyTo(destination);
            if (reverse)
                destination.Reverse();
        }

        /// <summary>
        /// The bytes appear in big-endian order, as a large hexadecimally encoded number.
        /// </summary>
        /// <returns></returns>
		public override string ToString() => Encoders.HexReverse.Encode(ReadOnlyBytes);

        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// Equivalent to ToHex.
        /// </summary>
        /// <returns></returns>
		public string ToStringFirstByteFirst() => Encoders.Hex.Encode(ReadOnlyBytes);

        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still appears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns></returns>
		public string ToHex() => Encoders.Hex.Encode(ReadOnlyBytes);

        public override int GetHashCode() => N0.GetHashCode() ^ N1.GetHashCode() ^ N2.GetHashCode();

        public override bool Equals(object obj) => obj is UInt160 int160 && this == int160;
        public bool Equals(UInt160 o) => N0 == o.N0 && N1 == o.N1 && N2 == o.N2;

        public static bool operator ==(UInt160 x, UInt160 y) => x.Equals(y);
        public static bool operator !=(UInt160 x, UInt160 y) => !(x == y);

        public static implicit operator UInt160(ReadOnlyByteSpan rhs)
        {
            return new UInt160(rhs);
        }

        public static implicit operator ReadOnlyByteSpan(UInt160 rhs)
        {
            return rhs.Bytes;
        }

        public int CompareTo(UInt160 o)
        {
            var r = N2.CompareTo(o.N2);
            if (r == 0) r = N1.CompareTo(o.N1);
            if (r == 0) r = N0.CompareTo(o.N0);
            return r;
        }
    }
}

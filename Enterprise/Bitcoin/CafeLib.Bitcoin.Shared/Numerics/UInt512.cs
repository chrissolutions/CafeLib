#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using System.Numerics;
using CafeLib.Bitcoin.Encode;
using CafeLib.Bitcoin.Global;
using CafeLib.Bitcoin.Shared.Numerics.Converters;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Shared.Numerics
{
    [JsonConverter(typeof(JsonConverterUInt512))]
    public struct UInt512
    {
        public UInt64 N0;
        private UInt64 _n1;
        private UInt64 _n2;
        private UInt64 _n3;
        private UInt64 _n4;
        private UInt64 _n5;
        private UInt64 _n6;
        private UInt64 _n7;

        static KzEncode _hex = KzEncoders.Hex;
        static KzEncode _hexRev = KzEncoders.HexReverse;

        public int Length => 64;

		public UInt512(UInt64 v0 = 0, UInt64 v1 = 0, UInt64 v2 = 0, UInt64 v3 = 0, UInt64 v4 = 0, UInt64 v5 = 0, UInt64 v6 = 0, UInt64 v7 = 0)
		{
            N0 = v0;
            _n1 = v1;
            _n2 = v2;
            _n3 = v3;
            _n4 = v4;
            _n5 = v5;
            _n6 = v6;
            _n7 = v7;
		}

        public UInt512(string hex, bool firstByteFirst = false) : this()
        {
            (firstByteFirst ? _hex : _hexRev).TryDecode(hex, Span);
        }

        public static UInt512 Zero { get; } = new UInt512(0);
        public static UInt512 One { get; } = new UInt512(1);


        public ReadOnlySpan<byte> ReadOnlySpan => Span;

        public Span<byte> Span {
            get {
                unsafe {
                    fixed (UInt64* p = &N0) {
                        byte* pb = (byte*)p;
                        var bytes = new Span<byte>(pb, 64);
                        return bytes;
                    }
                }
            }
        }

        public void Read(BinaryReader s)
        {
            s.Read(Span);
        }

        public BigInteger ToBigInteger() => new BigInteger(ReadOnlySpan);
        public byte[] ToBytes() => Span.ToArray();

        /// <summary>
        /// The bytes appear in big-endian order, as a large hexadecimally encoded number.
        /// </summary>
        /// <returns></returns>
		public override string ToString() => _hexRev.Encode(Span);
        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still apears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns></returns>
		public string ToStringFirstByteFirst() => _hex.Encode(Span);
        /// <summary>
        /// The bytes appear in little-endian order, first byte in memory first.
        /// But the high nibble, first hex digit, of the each byte still apears before the low nibble (big-endian by nibble order).
        /// </summary>
        /// <returns></returns>
		public string ToHex() => Kz.Hex.Encode(Span);

        public override int GetHashCode() => N0.GetHashCode() ^ _n1.GetHashCode() ^ _n2.GetHashCode() ^ _n3.GetHashCode();

        public override bool Equals(object obj) => obj is UInt512 && this == (UInt512)obj;
        public bool Equals(UInt512 o) => N0 == o.N0 && _n1 == o._n1 && _n2 == o._n2 && _n3 == o._n3 && _n4 == o._n4 && _n5 == o._n5 && _n6 == o._n6 && _n7 == o._n7;
        
        public static bool operator ==(UInt512 x, UInt512 y) => x.Equals(y);
        public static bool operator !=(UInt512 x, UInt512 y) => !(x == y);

        public static UInt512 operator ^(UInt512 x, UInt512 y)
        {
            var r = new UInt512
            {
                N0 = x.N0 ^ y.N0,
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

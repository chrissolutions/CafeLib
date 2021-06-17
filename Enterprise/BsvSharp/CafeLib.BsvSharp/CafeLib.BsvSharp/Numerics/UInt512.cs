#region Copyright
// Copyright (c) 2020 TonesNotes
// Copyright (c) 2021 ChrisSolutions
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using System.Numerics;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Numerics.Converters;
using Newtonsoft.Json;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.BsvSharp.Numerics
{
    [JsonConverter(typeof(JsonConverterUInt512))]
    public struct UInt512
    {
        private static readonly HexEncoder Hex = HexEncoder.Current;
        private static readonly HexReverseEncoder HexReverse = HexReverseEncoder.Current;
        private static readonly EndianEncoder Endian = EndianEncoder.Current;

        private ulong _n0;
        private ulong _n1;
        private ulong _n2;
        private ulong _n3;
        private ulong _n4;
        private ulong _n5;
        private ulong _n6;
        private ulong _n7;

        /// <summary>
        /// Length of UInt512.
        /// </summary>
        public const int Length = 64;

        public UInt512(byte[] bytes)
            : this()
        {
            if (bytes.Length < Length)
                throw new ArgumentException($"{Length} bytes are required.");

            bytes[..Length].CopyTo(Span);
            if (Endian.IsLittleEndian)
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

        public UInt512(string hex)
            : this()
        {
            Hex.Decode(hex).CopyTo(Span);
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
                        var span = new Span<byte>(pb, Length);
                        return span;
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
		public override string ToString() => HexReverse.Encode((byte[])Span);

        /// <summary>
        /// Get the Uint512 hash code.
        /// </summary>
        /// <returns></returns>
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

﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;

namespace CafeLib.Bitcoin.Encoding
{
    public static class Encoders
    {
        private static readonly Lazy<HexEncoder> LazyHex = new Lazy<HexEncoder>(() => new HexEncoder(), true);
        private static readonly Lazy<HexReverseEncoder> LazyHexReverse = new Lazy<HexReverseEncoder>(() => new HexReverseEncoder(), true);
        private static readonly Lazy<Utf8Encoder> LazyUtf8 = new Lazy<Utf8Encoder>(() => new Utf8Encoder());
        private static readonly Lazy<EndianEncoder> LazyEndian = new Lazy<EndianEncoder>(() => new EndianEncoder());

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// First byte first: The encoded string begins with the first byte.
        /// Character 0 corresponds to the high nibble of the first byte. 
        /// Character 1 corresponds to the low nibble of the first byte. 
        /// </summary>
        public static EndianEncoder Endian => LazyEndian.Value;

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// First byte first: The encoded string begins with the first byte.
        /// Character 0 corresponds to the high nibble of the first byte. 
        /// Character 1 corresponds to the low nibble of the first byte. 
        /// </summary>
        public static HexEncoder Hex => LazyHex.Value;

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// Last byte first: The encoded string begins with the last byte.
        /// Character 0 corresponds to the high nibble of the last byte. 
        /// Character 1 corresponds to the low nibble of the last byte. 
        /// </summary>
        public static HexReverseEncoder HexReverse => LazyHexReverse.Value;

        // <summary>
        // Base58 encoder.
        // </summary>
        public static Base58Encoder Base58 => Base58Encoder.Current;

        // <summary>
        // Base58 plus checksum encoder.
        // Checksum is first 4 bytes of double SHA256 hash of byte sequence.
        // Checksum is appended to byte sequence.
        // </summary>
        public static Base58CheckEncoder Base58Check => Base58CheckEncoder.Current;

        // <summary>
        // Base58 plus checksum encoder.
        // Checksum is first 4 bytes of double SHA256 hash of byte sequence.
        // Checksum is appended to byte sequence.
        // </summary>
        public static Utf8Encoder Utf8 => LazyUtf8.Value;

    }
}

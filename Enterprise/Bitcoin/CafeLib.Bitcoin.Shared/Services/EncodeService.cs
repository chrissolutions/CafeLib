#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Services
{
    internal class EncodeService : IEncodeService
    {
        public EncodeService()
        {
            Hex = new HexEncoder();
            HexReverse = new HexReverseEncoder();
        }

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// First byte first: The encoded string begins with the first byte.
        /// Character 0 corresponds to the high nibble of the first byte. 
        /// Character 1 corresponds to the low nibble of the first byte. 
        /// </summary>
        public HexEncoder Hex { get; }

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// Last byte first: The encoded string begins with the last byte.
        /// Character 0 corresponds to the high nibble of the last byte. 
        /// Character 1 corresponds to the low nibble of the last byte. 
        /// </summary>
        public HexReverseEncoder HexReverse { get; }

        /// <summary>
        /// Base58 encoder.
        /// </summary>
        //public static KzEncodeB58 B58 => lazyB58.Value;

        /// <summary>
        /// Base58 plus checksum encoder.
        /// Checksum is first 4 bytes of double SHA256 hash of byte sequence.
        /// Checksum is appended to byte sequence.
        /// </summary>
        //public static KzEncodeB58Check B58Check => lazyB58Check.Value;
    }

}

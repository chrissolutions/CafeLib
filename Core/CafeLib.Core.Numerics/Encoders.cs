using CafeLib.Core.Encodings;
using System;

namespace CafeLib.Core.Numerics
{
    internal static class Encoders
    {
        private static readonly Lazy<HexEncoder> LazyHex = new(() => new HexEncoder(), true);
        private static readonly Lazy<HexReverseEncoder> LazyHexReverse = new(() => new HexReverseEncoder(), true);

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
    }
}

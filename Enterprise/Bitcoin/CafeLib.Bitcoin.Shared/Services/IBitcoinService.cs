using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Network;

namespace CafeLib.Bitcoin.Shared.Services
{
    public interface IBitcoinService
    {
        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// First byte first: The encoded string begins with the first byte.
        /// Character 0 corresponds to the high nibble of the first byte. 
        /// Character 1 corresponds to the low nibble of the first byte. 
        /// </summary>
        HexEncoder Hex { get; }

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// Last byte first: The encoded string begins with the last byte.
        /// Character 0 corresponds to the high nibble of the last byte. 
        /// Character 1 corresponds to the low nibble of the last byte. 
        /// </summary>
        HexReverseEncoder HexReverse { get; }

        /// <summary>
        /// 
        /// </summary>
        BitcoinNetwork Network { get; }

        /// <summary>
        /// Base58 encoding prefix for public key addresses for the active network.
        /// </summary>
        ReadOnlyBytes PubkeyAddress { get; }

        /// <summary>
        /// Base58 encoding prefix for script addresses for the active network.
        /// </summary>
        ReadOnlyBytes ScriptAddress { get; } 

        /// <summary>
        /// Base58 encoding prefix for private keys for the active network.
        /// </summary>
        ReadOnlyBytes SecretKey { get; }

        /// <summary>
        /// Base58 encoding prefix for extended public keys for the active network.
        /// </summary>
        ReadOnlyBytes ExtPublicKey { get; } 

        /// <summary>
        /// Base58 encoding prefix for extended private keys for the active network.
        /// </summary>
        ReadOnlyBytes ExtSecretKey { get; }
    }
}

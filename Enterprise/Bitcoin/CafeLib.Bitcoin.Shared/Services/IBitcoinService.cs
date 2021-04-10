using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Network;

namespace CafeLib.Bitcoin.Shared.Services
{
    public interface IBitcoinService
    {
        /// <summary>
        /// 
        /// </summary>
        BitcoinNetwork Network { get; }

        /// <summary>
        /// Base58 encoding prefix for public key addresses for the active network.
        /// </summary>
        ReadOnlyByteSpan PubkeyAddress { get; }

        /// <summary>
        /// Base58 encoding prefix for script addresses for the active network.
        /// </summary>
        ReadOnlyByteSpan ScriptAddress { get; } 

        /// <summary>
        /// Base58 encoding prefix for private keys for the active network.
        /// </summary>
        ReadOnlyByteSpan SecretKey { get; }

        /// <summary>
        /// Base58 encoding prefix for extended public keys for the active network.
        /// </summary>
        ReadOnlyByteSpan ExtPublicKey { get; } 

        /// <summary>
        /// Base58 encoding prefix for extended private keys for the active network.
        /// </summary>
        ReadOnlyByteSpan ExtSecretKey { get; }
    }
}

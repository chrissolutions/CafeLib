using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Network;

namespace CafeLib.Bitcoin.Shared.Services
{
    public interface IBitcoinService
    {
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

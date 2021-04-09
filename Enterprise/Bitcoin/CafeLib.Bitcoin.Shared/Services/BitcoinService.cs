using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Network;

namespace CafeLib.Bitcoin.Shared.Services
{
    public class BitcoinService : IBitcoinService
    {
        public BitcoinNetwork Network { get; }

        /// <summary>
        /// BitcoinService constructor.
        /// </summary>
        /// <param name="network"></param>
        public BitcoinService(NetworkType networkType)
        {
            //Network = networkType switch
            //{
            //    NetworkType.Main => new    
            //}
        }

        /// <summary>
        /// Base58 encoding prefix for public key addresses for the active network.
        /// </summary>
        public ReadOnlyBytes PubkeyAddress => Network.Base58Prefix(Base58Type.PubkeyAddress);

        public ReadOnlyBytes ScriptAddress => Network.Base58Prefix(Base58Type.ScriptAddress);

        public ReadOnlyBytes SecretKey => Network.Base58Prefix(Base58Type.SecretKey);

        public ReadOnlyBytes ExtPublicKey => Network.Base58Prefix(Base58Type.ExtPublicKey);

        public ReadOnlyBytes ExtSecretKey => Network.Base58Prefix(Base58Type.ExtSecretKey);
    }
}

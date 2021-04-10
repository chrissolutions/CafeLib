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
        /// <param name="networkType"></param>
        public BitcoinService(NetworkType networkType)
        {
            Network = networkType switch
            {
                NetworkType.Unknown => new MainNetwork(),
                NetworkType.Main => new MainNetwork(),
                NetworkType.Test => throw new System.NotImplementedException(),
                NetworkType.Regression => throw new System.NotImplementedException(),
                NetworkType.Scaling => throw new System.NotImplementedException(),
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// Base58 encoding prefix for public key addresses for the active network.
        /// </summary>
        public ReadOnlyByteSpan PubkeyAddress => Network.Base58Prefix(Base58Type.PubkeyAddress);

        public ReadOnlyByteSpan ScriptAddress => Network.Base58Prefix(Base58Type.ScriptAddress);

        public ReadOnlyByteSpan SecretKey => Network.Base58Prefix(Base58Type.SecretKey);

        public ReadOnlyByteSpan ExtPublicKey => Network.Base58Prefix(Base58Type.ExtPublicKey);

        public ReadOnlyByteSpan ExtSecretKey => Network.Base58Prefix(Base58Type.ExtSecretKey);
    }
}

using System;
using CafeLib.Bitcoin.Shared.Network;

namespace CafeLib.Bitcoin.Shared.Services
{
    public static class RootService
    {
        private static IBitcoinNetwork _bitcoinNetwork;
        private static readonly object Mutex = new object();

        public const string MasterBip32Key = "Bitcoin seed";

        public static void Initialize(NetworkType networkType)
        {
            if (_bitcoinNetwork != null) throw new InvalidOperationException();

            lock (Mutex)
            {
                _bitcoinNetwork = networkType switch
                {
                    NetworkType.Main => new MainNetwork(),
                    NetworkType.Test => new TestNetwork(),
                    NetworkType.Regression => new RegressionTestNetwork(),
                    NetworkType.Scaling => new ScalingTestNetwork(),
                    _ => _bitcoinNetwork
                };
            }
        }

        public static IBitcoinNetwork Network => _bitcoinNetwork ?? throw new InvalidOperationException();
    }
}

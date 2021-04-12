using System;
using CafeLib.Bitcoin.Shared.Network;

namespace CafeLib.Bitcoin.Shared.Services
{
    public static class RootService
    {
        private static BitcoinNetwork _bitcoinNetwork;
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
                    NetworkType.Test => throw new NotImplementedException(),
                    NetworkType.Regression => throw new NotImplementedException(),
                    NetworkType.Scaling => throw new NotImplementedException(),
                    _ => throw new NotImplementedException()
                };
            }
        }

        public static IBitcoinNetwork Network => _bitcoinNetwork ?? throw new InvalidOperationException();
    }
}

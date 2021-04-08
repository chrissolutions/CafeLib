#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Builder;
using CafeLib.Bitcoin.Encode;

namespace CafeLib.Bitcoin.Global
{
    public static class Kz
    {
        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// First byte first: The encoded string begins with the first byte.
        /// Character 0 corresponds to the high nibble of the first byte. 
        /// Character 1 corresponds to the low nibble of the first byte. 
        /// </summary>
        public static KzEncodeHex Hex => KzEncoders.Hex;

        /// <summary>
        /// Encodes a sequence of bytes as hexadecimal digits where:
        /// Last byte first: The encoded string begins with the last byte.
        /// Character 0 corresponds to the high nibble of the last byte. 
        /// Character 1 corresponds to the low nibble of the last byte. 
        /// </summary>
        public static KzEncodeHexReverse HexR => KzEncoders.HexReverse;

        public static string MasterBip32Key = "Bitcoin seed";

        public static KzBScript Script() => new KzBScript();
        public static KzBTransaction Tx() => new KzBTransaction();

        /// <summary>
        /// Base58 encoding prefix for public key addresses for the active network.
        /// </summary>
        public static ReadOnlySpan<byte> PubkeyAddress => Params.Base58Prefix(KzBase58Type.PubkeyAddress);
        /// <summary>
        /// Base58 encoding prefix for script addresses for the active network.
        /// </summary>
        public static ReadOnlySpan<byte> ScriptAddress => Params.Base58Prefix(KzBase58Type.ScriptAddress);
        /// <summary>
        /// Base58 encoding prefix for private keys for the active network.
        /// </summary>
        public static ReadOnlySpan<byte> SecretKey => Params.Base58Prefix(KzBase58Type.SecretKey);
        /// <summary>
        /// Base58 encoding prefix for extended public keys for the active network.
        /// </summary>
        public static ReadOnlySpan<byte> ExtPublicKey => Params.Base58Prefix(KzBase58Type.ExtPublicKey);
        /// <summary>
        /// Base58 encoding prefix for extended private keys for the active network.
        /// </summary>
        public static ReadOnlySpan<byte> ExtSecretKey => Params.Base58Prefix(KzBase58Type.ExtSecretKey);

        public static void CreateChainParams(KzChain chain)
        {
            lock (chainLock) {
                if (Kz.chain != KzChain.Unknown)
                    throw new InvalidOperationException("The chain has already been selected.");
                Kz.chain = chain;
            }
        }

        private static KzChainParams CreateChainParams()
        {
            var chain = KzChain.Unknown;
            lock (chainLock) {
                if (Kz.chain == KzChain.Unknown) Kz.chain = KzChain.Main;
                chain = Kz.chain;
            }
            return chain switch
            {
                KzChain.Main => new KzMainParams(),
                KzChain.Regtest => new KzRegTestParams(),
                KzChain.Stn => new KzStnParams(),
                KzChain.Test => new KzTestNetParams(),
                _ => (KzChainParams)null
            };
        }

        static object chainLock = new object();
        static KzChain chain = KzChain.Unknown;
        static Lazy<KzChainParams> lazyParams;

        static Kz()
        {
            lazyParams = new Lazy<KzChainParams>(() => CreateChainParams(), true);
        }

        public static KzChainParams Params => lazyParams.Value;

        public static KzConsensus Consensus => Params.Consensus;
    }
}

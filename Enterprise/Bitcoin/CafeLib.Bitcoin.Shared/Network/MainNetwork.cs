﻿using System;
using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Network
{
    public class MainNetwork : BitcoinNetwork
    {
        public MainNetwork()
            : base(NetworkType.Main, new Lazy<byte[][]>(GetPrefixes).Value)
        {
            //Consensus = new KzConsensus
            //{
            //    nSubsidyHalvingInterval = 210000,
            //    BIP34Height = 227931,
            //    BIP34Hash = new KzUInt256("000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
            //    // 000000000000000004c2b624ed5d7756c508d90fd0da2c7c679febfa6c4735f0
            //    BIP65Height = 388381,
            //    // 00000000000000000379eaa19dce8c9b722d46ae6a57c2f1a988119488b50931
            //    BIP66Height = 363725,
            //    powLimit = new KzUInt256("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"),
            //    // two weeks
            //    nPowTargetTimespan = 14 * 24 * 60 * 60,
            //    nPowTargetSpacing = 10 * 60,
            //    fPowAllowMinDifficultyBlocks = false,
            //    fPowNoRetargeting = false,
            //    // 95% of 2016
            //    nRuleChangeActivationThreshold = 1916,
            //    // nPowTargetTimespan / nPowTargetSpacing
            //    nMinerConfirmationWindow = 2016,

            //    // The best chain should have at least this much work.
            //    nMinimumChainWork = new KzUInt256("000000000000000000000000000000000000000000a0f3064330647e2f6c4828"),

            //    // By default assume that the signatures in ancestors of this block are valid.
            //    defaultAssumeValid = new KzUInt256("000000000000000000e45ad2fbcc5ff3e85f0868dd8f00ad4e92dffabe28f8d2"),

            //    // August 1, 2017 hard fork
            //    uahfHeight = 478558,

            //    // November 13, 2017 hard fork
            //    daaHeight = 504031,
            //    vDeployments =
            //    {
            //        [(int) KzDeploymentPos.DEPLOYMENT_TESTDUMMY] = new KzBIP9Deployment()
            //        {
            //            bit = 28,
            //            nStartTime = 1199145601, // January 1, 2008
            //            nTimeout = 1230767999 // December 31, 2008
            //        },
            //        [(int) KzDeploymentPos.DEPLOYMENT_CSV] = new KzBIP9Deployment()
            //        {
            //            bit = 0,
            //            nStartTime = 1462060800, // May 1st, 2016
            //            nTimeout = 1493596800 // May 1st, 2017
            //        }
            //    },
            //};
        }

        private static byte[][] GetPrefixes()
        {
            // Deployment of BIP68, BIP112, and BIP113.
            var prefixes = new byte[(int)Base58Type.MaxBase58Types][];
            prefixes[(int)Base58Type.PubkeyAddress] = new byte[] { (0) };
            prefixes[(int)Base58Type.ScriptAddress] = new byte[] { (5) };
            prefixes[(int)Base58Type.SecretKey] = new byte[] { (128) };
            prefixes[(int)Base58Type.ExtPublicKey] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
            prefixes[(int)Base58Type.ExtSecretKey] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
            return prefixes;
        }
    }
}
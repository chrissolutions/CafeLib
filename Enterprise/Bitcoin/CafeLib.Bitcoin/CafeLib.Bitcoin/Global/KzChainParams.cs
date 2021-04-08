#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;

namespace CafeLib.Bitcoin.Global
{
    public class KzChainParams
    {
        public KzConsensus Consensus { get; protected set; }

        public string NetworkId { get; protected set; }

        protected byte[][] Base58Prefixes { get; }

        protected KzChainParams()
        {
            Base58Prefixes = new byte[(int)KzBase58Type.MaxBase58Types][];
        }

        public ReadOnlySpan<byte> Base58Prefix(KzBase58Type type) => Base58Prefixes[(int)type].AsSpan();
    }

}

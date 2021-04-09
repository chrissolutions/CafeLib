#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Core.Extensions;

namespace CafeLib.Bitcoin.Shared.Network
{
    public abstract class BitcoinNetwork
    {
        //public KzConsensus Consensus { get; protected set; }

        public string NetworkId { get; }

        protected byte[][] Base58Prefixes { get; }

        protected BitcoinNetwork(NetworkType nodeType, byte[][] base58Prefixes)
        {
            NetworkId = nodeType.GetDescriptor();
            Base58Prefixes = base58Prefixes;
        }

        public ReadOnlySpan<byte> Base58Prefix(Base58Type type) => Base58Prefixes[(int)type].AsSpan();
    }
}

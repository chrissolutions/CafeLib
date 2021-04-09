#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Node
{
    public class NodeMode
    {
        //public KzConsensus Consensus { get; protected set; }

        public NodeType NodeType { get; }

        protected byte[][] Base58Prefixes { get; }

        protected NodeMode(NodeType nodeType, byte[][] base58Prefixes)
        {
            NodeType = nodeType;
            Base58Prefixes = base58Prefixes;
        }

        public ReadOnlySpan<byte> Base58Prefix(Base58Type type) => Base58Prefixes[(int)type].AsSpan();
    }
}

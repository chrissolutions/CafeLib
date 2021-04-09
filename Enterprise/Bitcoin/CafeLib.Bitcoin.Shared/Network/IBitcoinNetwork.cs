#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Network
{
    public interface IBitcoinNetwork
    {
        //public KzConsensus Consensus { get; protected set; }

        string NetworkId { get; }

        ReadOnlyBytes Base58Prefix(Base58Type type);
    }
}

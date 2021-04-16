#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Persistence;

namespace CafeLib.Bitcoin.Shared.Chain
{
    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction input's previous output reference as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// </summary>
    public struct OutPoint
    {
        public UInt256 Txid { get; private set; }

        public uint Index { get; private set; }

        public OutPoint(UInt256 hashTx, uint index)
        {
            Txid = hashTx; 
            Index = index;
        }

        public bool TryReadOutpoint(ref SequenceReader<byte> r)
        {
            var txid = Txid;

            if (!r.TryCopyToA(ref txid) || !r.TryReadLittleEndian(out uint index)) return false;

            Txid = txid;
            Index = index;
            return true;
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer.Add(Txid).Add(Index);
            return writer;
        }
    }
}

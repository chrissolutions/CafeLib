#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;

namespace CafeLib.Bitcoin.Chain
{
    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction input's previous output reference as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// </summary>
    public struct OutPoint
    {
        public UInt256 Txid { get; private set; }

        public int Index { get; private set; }

        public OutPoint(UInt256 hashTx, int index)
        {
            Txid = hashTx; 
            Index = index;
        }

        public bool TryReadOutPoint(ref ByteSequenceReader r)
        {
            var txid = Txid;

            if (!r.TryReadUInt256(ref txid) || !r.TryReadLittleEndian(out int index)) return false;

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

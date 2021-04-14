#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
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
        private UInt256 _hashTx;
        Int32 _n;

        public UInt256 HashTx => _hashTx;

        public Int32 N => _n;

        public OutPoint(UInt256 hashTx, Int32 n)
        {
            _hashTx = hashTx; 
            _n = n;
        }

        public bool TryReadOutPoint(ref SequenceReader<byte> r)
        {
            if (!r.TryCopyToA(ref _hashTx)) goto fail;
            if (!r.TryReadLittleEndian(out _n)) goto fail;

            return true;
        fail:
            return false;
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer.Add(_hashTx).Add(_n);
            return writer;
        }
    }
}

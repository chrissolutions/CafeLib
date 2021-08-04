﻿using System.Collections.Generic;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Transactions;

namespace CafeLib.BsvSharp.Chain
{
    public static class MerkleExtensions
    {
        public static UInt256 ComputeMerkleRoot(this IEnumerable<Transaction> txs)
        {
            using var mt = new MerkleTree();
            foreach (var tx in txs)
            {
                mt.AddHash(tx.TxHash);
            }
            return mt.GetMerkleRoot();
        }
    }
}
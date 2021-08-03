using System.Collections.Generic;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public static class MerkleExtensions
    {
        public static UInt256 ComputeMerkleRoot(this IEnumerable<Transaction> txs)
        {
            using var mt = new MerkleTree();
            foreach (var tx in txs)
            {
                mt.AddHash(tx.Hash);
            }
            return mt.GetMerkleRoot();
        }
    }
}

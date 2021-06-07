#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Chain
{
    /// <summary>
    /// 
    /// This algorithm is incrementally efficient, the worst case cost of obtaining an incremental root hash
    /// is O(tree_height), not O(tx_count * log(tree_height)).
    /// 
    /// There is no protection currently from CVE-2012-2459 vulnerability (duplicated pairs of transactions).
    ///
    /// </summary>
    public class MerkleTree : IDisposable
    {
        private bool _disposed;

        private long _count;
        private readonly List<MerkleTreeNode> _nodes = new List<MerkleTreeNode>();
        private readonly SHA256 _sha256;

        public MerkleTree()
        {
            _sha256 = SHA256.Create();
        }

        public void AddTransactions(IEnumerable<Transaction> txs)
        {
            foreach (var tx in txs) 
                AddTransaction(tx);
        }

        public static UInt256 ComputeMerkleRoot(IEnumerable<Transaction> txs)
        {
            using var mt = new MerkleTree();
            mt.AddTransactions(txs);
            return mt.GetMerkleRoot();
        }

        /// <summary>
        /// Compute the full merkle tree root hash from the incremental state.
        /// Typically called after adding all the available transactions.
        /// Propagates hashes upwards from incomplete subtrees by copying left subtree hash when needed.
        /// Note that this copying leads to a vulnerability: CVE-2012-2459
        /// </summary>
        /// <returns></returns>
        public UInt256 ComputeHashMerkleRoot()
        {
            if (_count == 0)
                return UInt256.Zero;

            var node = _nodes[0];

            if (_count == 1)
                return node.LeftHash;

            Debug.Assert(!_nodes.Last().HasBoth);

            // Skip complete subtrees...
            while (node.HasBoth) node = node.Parent;

            // If only the last node is incomplete then
            // the whole left subtree is complete,
            // and there's nothing in the right subtree.
            if (node.Parent == null)
                return node.LeftHash;

            // Don't alter incremental state of tree hashes when computing partial results.
            var hasBoth = false;

            UInt256 newHash;

            do
            {
                if (!hasBoth)
                    node.LeftHash.Span.CopyTo(node.RightHash.Span);

                newHash = ComputeHash(node);
                var np = node.Parent;
                if (np != null)
                {
                    if (node.IsLeftOfParent)
                    {
                        np.LeftHash = newHash;
                        hasBoth = false;
                    }
                    else
                    {
                        np.RightHash = newHash;
                        hasBoth = true;
                    }
                }
                node = np;
            }
            while (node != null);

            return newHash;
        }

        public UInt256 GetMerkleRoot()
        {
            return ComputeHashMerkleRoot();
        }

        #region IDisposable

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            try
            {
                _sha256.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Update the incremental state by one additional transaction hash.
        /// This creates at most one MerkleTreeNode per level of the tree.
        /// These are reused as subtrees fill up.
        /// </summary>
        /// <param name="tx"></param>
        private void AddTransaction(Transaction tx)
        {
            _count++;
            var newHash = tx.Hash;
            if (_count == 1)
            {
                // First transaction.
                _nodes.Add(new MerkleTreeNode(newHash, null));
            } 
            else
            {
                var n = _nodes[0];
                if (n.HasBoth)
                {
                    // Reuse previously filled nodes.
                    var n0 = n;
                    while (n?.HasBoth == true)
                    {
                        n.RightOfParent = !n.RightOfParent;
                        n.HasRight = false;
                        n.HasLeft = false;
                        n = n.Parent;
                    }
                    n0.SetLeftHash(newHash);
                }
                else
                {
                    // Complete leaf node, compute completed hashes and propagate upwards.
                    n.SetRightHash(newHash);
                    do
                    {
                        newHash = ComputeHash(n);
                        var np = n.Parent;
                        if (np == null)
                        {
                            _nodes.Add(new MerkleTreeNode(newHash, n));
                            break;
                        }
                        if (n.IsLeftOfParent)
                            np.SetLeftHash(newHash);
                        else
                            np.SetRightHash(newHash);
                        n = np;
                    } while (n.HasBoth);
                }
            }
        }

        private static UInt256 ComputeHash(MerkleTreeNode node)
        {
            // This ToArray call could be eliminated.
            var h = new UInt256();
            Hashes.Hash256(node.LeftRightHashes, h);
            return h;
        }

        #endregion
    }
}
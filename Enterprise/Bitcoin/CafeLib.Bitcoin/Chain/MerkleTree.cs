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
        public static UInt256 ComputeMerkleRoot(IEnumerable<Transaction> txs)
        {
            using var mt = new MerkleTree();
            mt.AddTransactions(txs);
            return mt.GetMerkleRoot();
        }

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

        /// <summary>
        /// Update the incremental state by one additional transaction hash.
        /// This creates at most one MerkleTreeNode per level of the tree.
        /// These are reused as subtrees fill up.
        /// </summary>
        /// <param name="tx"></param>
        void AddTransaction(Transaction tx)
        {
            _count++;
            var newHash = tx.Hash;
            if (_count == 1)
            {
                // First transaction.
                _nodes.Add(new MerkleTreeNode(newHash, null));
            } else
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

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _sha256.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~KzMerkleTree() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
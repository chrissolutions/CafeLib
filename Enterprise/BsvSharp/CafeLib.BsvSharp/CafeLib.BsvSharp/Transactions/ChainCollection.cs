using System.Collections.ObjectModel;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    ///	Collection of vertices.
    /// </summary>
    public class ChainCollection<T> : KeyedCollection<UInt256, T> where T : IChainId
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">item to be added to the collection item</param>
        public new void Add(T item)
        {
            if (!Contains(item))
                base.Add(item);
        }

        /// <summary>
        /// Determines whether the collection contains the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Contains(T item)
        {
            return Contains(item.Hash);
        }

        /// <summary>
        /// Remove item from collection
        /// </summary>
        /// <param name="item">collection item</param>
        /// <returns></returns>
        public new bool Remove(T item)
        {
            return Contains(item.Hash) && Remove(item.Hash);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Return the key of the keyed item.
        /// </summary>
        /// <param name="item">collection item</param>
        /// <returns>the key of the item</returns>
        protected override UInt256 GetKeyForItem(T item)
        {
            return item.Hash;
        }

        #endregion
    }

    public class TxInCollection : ChainCollection<TransactionInput>
    {
    }

    public class TxOutCollection : ChainCollection<TransactionOutput>
    {
    }

    public class TxCollection : ChainCollection<Transaction>
    {
    }
}
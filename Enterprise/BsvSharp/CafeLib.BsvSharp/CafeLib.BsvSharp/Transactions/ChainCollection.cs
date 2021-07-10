﻿using System.Collections.Generic;
using System.Security.Claims;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    ///	Collection of vertices.
    /// </summary>
    public class ChainCollection<T> : List<T> where T : IChainId
    {
        public ChainCollection()
        {
        }

        public ChainCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }
    }

    public class TxInCollection : ChainCollection<TxIn>
    {
        public TxInCollection()
        {
        }

        public TxInCollection(IEnumerable<TxIn> collection)
            : base(collection)
        {
        }
    }

    public class TxOutCollection : ChainCollection<TxOut>
    {
        public TxOutCollection()
        {
        }

        public TxOutCollection(IEnumerable<TxOut> collection)
            : base(collection)
        {
        }
    }

    public class TxCollection : ChainCollection<Transaction>
    {
        public TxCollection()
        {
        }

        public TxCollection(IEnumerable<Transaction> collection)
            : base(collection)
        {
        }
    }
}
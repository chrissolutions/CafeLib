using System.Collections.Generic;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    ///	Collection of vertices.
    /// </summary>
    public class ChainCollection<T> : List<T> where T : IChainId
    {
    }

    public class TxInCollection : ChainCollection<TxIn>
    {
    }

    public class TxOutCollection : ChainCollection<TransactionOutput>
    {
    }

    public class TxCollection : ChainCollection<Transaction>
    {
    }
}
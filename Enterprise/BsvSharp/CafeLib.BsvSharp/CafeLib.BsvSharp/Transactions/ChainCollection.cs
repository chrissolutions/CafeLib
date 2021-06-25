using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public class ChainCollection<T> : IEnumerable<T> where T : IChainId
    {
        private readonly IDictionary<UInt256, T> _map = new ConcurrentDictionary<UInt256, T>();

        public IEnumerator<T> GetEnumerator() => _map.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(T item) => _map.Add(item.Hash, item);
        public void Clear() => _map.Clear();
        public bool Contains(T item) => _map.ContainsKey(item.Hash);
        public void CopyTo(T[] array, int arrayIndex) => _map.Values.CopyTo(array, arrayIndex);
        public bool Remove(T item) => _map.Remove(item.Hash);
        public T this[UInt256 key] => _map[key];
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

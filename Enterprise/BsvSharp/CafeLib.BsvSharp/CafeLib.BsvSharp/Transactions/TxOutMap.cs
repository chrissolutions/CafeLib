using System.Collections.Generic;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Extensions;

namespace CafeLib.BsvSharp.Transactions
{
    public class TxOutMap
    {
        private readonly IDictionary<string, TxOut> _map = new Dictionary<string, TxOut>();

        public TxOut Get(UInt256 txHash, int txOutIndex)
        {
            var key = $"{txHash}:{txOutIndex}";
            return _map[key];
        }

        public void Set(UInt256 txHash, int txOutIndex, TxOut txOut)
        {
            var key = $"{txHash}:{txOutIndex}";
            _map.AddOrUpdate(key, txOut, (k, v) => txOut);
        }

        public void SetTransaction(Transaction tx)
        {
            tx.Outputs.ForEach((x, i) => Set(tx.Hash, i, x));
        }
    }
}

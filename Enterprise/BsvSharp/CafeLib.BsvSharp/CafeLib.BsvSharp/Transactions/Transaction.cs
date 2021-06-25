using System;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public class Transaction : IChainId
    {
        public int Version { get; private set; } = 1;

        public int LockTime { get; private set; } = 0;

        public TxInCollection  Inputs { get; private set; } //this transaction's inputs
        public TxOutCollection Outputs { get; private set; } //this transaction's outputs
        public TxOutCollection Utxo { get; private set; }  //the UTXOs from spent Transaction

        //Address _changeAddress;
        //LockingScriptBuilder _changeScriptBuilder;
        //final Set<TransactionOption> _transactionOptions = Set<TransactionOption>();

        public UInt256 Hash { get; private set; }
    }
}

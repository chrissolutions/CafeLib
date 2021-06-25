using System;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public class Transaction
    {
        public int Version { get; private set; } = 1;

        public int LockTime { get; private set; } = 0;

        public TransactionInput[]  Inputs { get; private set; } //this transaction's inputs
        public TransactionOutput[] Outputs { get; private set; } //this transaction's outputs
        public TransactionOutput[] Utxo { get; private set; }  //the UTXOs from spent Transaction

        //Address _changeAddress;
        //LockingScriptBuilder _changeScriptBuilder;
        //final Set<TransactionOption> _transactionOptions = Set<TransactionOption>();

        public UInt256 Hash { get; private set; }
        String _txId;
    }
}

using System;
using System.Linq;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Transactions
{
    public class Transaction : IChainId
    {
        public string Id => Encoders.HexReverse.Encode(Hash);
        public UInt256 Hash { get; private set; }
        public int Version { get; private set; } = 1;
        public int LockTime { get; private set; }
        public Amount Fee { get; private set; }

        public TxInCollection  Inputs { get; private set; } //this transaction's inputs
        public TxOutCollection Outputs { get; private set; } //this transaction's outputs
        public TxOutCollection Utxo { get; private set; }  //the UTXOs from spent Transaction

        //if we have a Transaction with one input, and a prevTransactionId of zeroooos, it's a coinbase.
        public bool IsCoinbase => Inputs.Count == 1 && Inputs.First().Hash == UInt256.Zero;

        //
        // _changeAddress;
        //LockingScriptBuilder _changeScriptBuilder;

        //bool _changeScriptFlag = false;


        public TransactionOption Option { get; private set; }

        public Transaction()
        {
        }

        public Transaction(
            int version, 
            TxInCollection vin, 
            TxOutCollection vout, 
            int lockTime,
            long fee = 0L,
            TransactionOption option = TransactionOption.DisableAll)
        {
            Version = version;
            Inputs = vin;
            Outputs = vout;
            LockTime = lockTime;
            Fee = new Amount(fee);
            Option = option;
        }

    }
}

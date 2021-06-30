using System;
using System.Linq;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
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
        public Address ChangeAddress { get; private set; }

        public TxInCollection  Inputs { get; private set; } //this transaction's inputs
        public TxOutCollection Outputs { get; private set; } //this transaction's outputs
        public TxOutCollection Utxo { get; private set; }  //the UTXOs from spent Transaction

        //if we have a Transaction with one input, and a prevTransactionId of zeroooos, it's a coinbase.
        public bool IsCoinbase => Inputs.Count == 1 && Inputs.First().Hash == UInt256.Zero;

        //LockingScriptBuilder _changeScriptBuilder;

        //bool _changeScriptFlag = false;


        public TransactionOption Option { get; private set; }

        public Transaction()
        {
        }

        public Transaction
        (
            int version, 
            TxInCollection vin, 
            TxOutCollection vout, 
            int lockTime,
            long fee = 0L,
            TransactionOption option = TransactionOption.DisableAll
        )
        {
            Version = version;
            Inputs = vin;
            Outputs = vout;
            LockTime = lockTime;
            Fee = new Amount(fee);
            Option = option;
        }

        public Transaction SpendTo(Address recipient, Amount sats, Script? lockingScript = null)
        {
            if (sats <= Amount.Zero) throw new ArgumentException("You can only spend a positive amount of satoshis");

            lockingScript ??= new P2PkhScriptBuilder(recipient).ToScript();

            var txOut = new TransactionOutput();

            //var txnOutput = TransactionOutput((scriptBuilder: scriptBuilder);
            ////        txnOutput.recipient = recipient;
            //            txnOutput.satoshis = sats;
            ////        txnOutput.script = scriptBuilder.getScriptPubkey();

            return AddOutput(txOut);
        }

        public Transaction AddOutput(TransactionOutput txOutput)
        {
            Outputs.Add(txOutput);
            UpdateChangeOutput();
            return this;
        }

        #region Helpers

        private void UpdateChangeOutput()
        {
            if (ChangeAddress == null) return;

            //if (_changeScriptBuilder == null) return;

            //_removeChangeOutputs();

            //if (_nonChangeRecipientTotals() == _inputTotals()) return;

            //var txnOutput = getChangeOutput(_changeScriptBuilder);

            //var changeAmount = _recalculateChange();

            ////can't spend negative amount of change :/
            //if (changeAmount > BigInt.zero)
            //{
            //    txnOutput.satoshis = changeAmount;
            //    txnOutput.script = _changeScriptBuilder.getScriptPubkey();
            //    txnOutput.isChangeOutput = true;
            //    _txnOutputs.add(txnOutput);
            //}
        }

        #endregion

    }
}

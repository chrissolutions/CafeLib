using System;
using System.Linq;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;
using CafeLib.Core.Extensions;

namespace CafeLib.BsvSharp.Transactions
{
    public class Transaction : IChainId
    {
        private ScriptBuilder _changeScriptBuilder;
        private bool _changeScriptFlag = false;

        public string Id => Encoders.HexReverse.Encode(Hash);
        public UInt256 Hash { get; private set; }
        public int Version { get; private set; } = 1;
        public int LockTime { get; private set; }
        public Amount Fee { get; private set; }
        public Address ChangeAddress { get; private set; }

        public TxInCollection  Inputs { get; private set; } //this transaction's inputs
        public TxOutCollection Outputs { get; private set; } //this transaction's outputs
        public TxOutCollection Utxo { get; private set; }  //the UTXOs from spent Transaction

        //if we have a Transaction with one input, and a prevTransactionId of zero, it's a coinbase.
        public bool IsCoinbase => Inputs.Count == 1 && Inputs[0].Hash == UInt256.Zero;

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

        public Transaction SpendTo(Address recipient, Amount sats, ScriptBuilder scriptBuilder = null)
        {
            if (sats <= Amount.Zero) throw new ArgumentException("You can only spend a positive amount of satoshis");

            scriptBuilder ??= new P2PkhScriptBuilder(recipient);
            var txOut = new TxOut(Hash, Outputs.Count, sats, scriptBuilder);
            return AddOutput(txOut);
        }

        /// <summary>
        /// Add a "change" output to this transaction
        ///
        /// When a new transaction is created to spend coins from an input transaction,
        /// the entire *UTXO* needs to be consumed. I.e you cannot *partially* spend coins.
        /// What needs to happen is :
        ///   1) You consumer the entire UTXO in the new transaction input
        ///   2) You subtract a *change* amount from the UTXO and the remainder will be sent to the receiving party
        ///
        /// The change amount is automatically calculated based on the fee rate that you set with [withFee()] or [withFeePerKb()]
        ///
        /// [changeAddress] - A bitcoin address where a standard P2PKH (Pay-To-Public-Key-Hash) output will be "sent"
        ///
        /// [scriptBuilder] - A [LockingScriptBuilder] that will be used to create the locking script (scriptPubKey) for the [TransactionOutput].
        ///                   A null value results in a [P2PKHLockBuilder] being used by default, which will create a Pay-to-Public-Key-Hash output script.
        ///
        /// Returns an instance of the current Transaction as part of the builder pattern.
        /// 
        /// </summary>
        /// <param name="changeAddress"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Transaction SendChangeTo(Address changeAddress, ScriptBuilder scriptBuilder = null)
        {
            scriptBuilder ??= new P2PkhScriptBuilder(changeAddress);

            _changeScriptFlag = true;
            //get fee, and if there is not enough change to cover fee, remove change outputs

            //delete previous change transaction if exists
            ChangeAddress = changeAddress;
            _changeScriptBuilder = scriptBuilder;
            UpdateChangeOutput();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txOutput"></param>
        /// <returns></returns>
        public Transaction AddOutput(TxOut txOutput)
        {
            Outputs.Add(txOutput);
            UpdateChangeOutput();
            return this;
        }

        public TxOut GetChangeOutput(ScriptBuilder changeBuilder)
        {
            var txOut = Outputs.SingleOrDefault( x => x.IsChangeOutput);

            if (txOut.TxHash == TxOut.Null.TxHash)
            {
                txOut = new TxOut();      //new TxOut(changeBuilder);
                txOut.IsChangeOutput = true;
                Outputs.Add(txOut);
            }

            return txOut;
        }


        #region Helpers

        private void UpdateChangeOutput()
        {
            if (ChangeAddress == null) return;

            if (_changeScriptBuilder == null) return;

            RemoveChangeOutputs();

            if (NonChangeRecipientTotals() == InputTotals()) return;

            var txOut = GetChangeOutput(_changeScriptBuilder);

            var changeAmount = RecalculateChange();

            ////can't spend negative amount of change :/
            if (changeAmount > Amount.Zero)
            {
                //txOut.Amount = changeAmount;
                //tnOut.script = _changeScriptBuilder.getScriptPubkey();
                //tnOut.isChangeOutput = true;
                //_txnOutputs.add(txnOutput);
            }
        }

        private void RemoveChangeOutputs() => Outputs.Where(x => x.IsChangeOutput).ForEach(x => Outputs.Remove(x));
        
        private Amount NonChangeRecipientTotals() => 
            Outputs
                .Where(txOut => !txOut.IsChangeOutput)
                .Aggregate(Amount.Zero, (prev, x) => prev + x.Amount);

        private Amount RecipientTotals() => Outputs.Aggregate(Amount.Zero, (prev, x) => prev + x.Amount);

        private Amount InputTotals() => Inputs.Aggregate(Amount.Zero, (prev, x) => prev + x.Amount);

        private Amount RecalculateChange()
        {
            //var inputAmount = _inputTotals();
            //var outputAmount = _nonChangeRecipientTotals();
            //var unspent = inputAmount - outputAmount;

            //return unspent - getFee();

            return Amount.Zero;
        }


        #endregion

    }
}

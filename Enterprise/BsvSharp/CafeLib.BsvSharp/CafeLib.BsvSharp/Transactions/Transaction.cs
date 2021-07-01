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
        private bool _hasChangeScript = false;
        private Amount _fee = Amount.Null;

        public string Id => Encoders.HexReverse.Encode(Hash);
        public UInt256 Hash { get; private set; }
        public int Version { get; private set; } = 1;
        public int LockTime { get; private set; }
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
            _fee = new Amount(fee);
            Option = option;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="sats"></param>
        /// <param name="scriptBuilder"></param>
        /// <returns></returns>
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
        /// <param name="scriptBuilder"></param>
        /// <returns></returns>
        public Transaction SendChangeTo(Address changeAddress, ScriptBuilder scriptBuilder = null)
        {
            scriptBuilder ??= new P2PkhScriptBuilder(changeAddress);

            _hasChangeScript = true;
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

            if (txOut == TxOut.Null)
            {
                txOut = new TxOut(Hash, Outputs.Count, changeBuilder, true);
                Outputs.Add(txOut);
            }

            return txOut;
        }
        
        ///  Calculates the fee of the transaction.
        ///
        ///  If there's a fixed fee set, return that.
        ///
        ///  If there is no change output set, the fee is the
        ///  total value of the outputs minus inputs. Note that
        ///  a serialized transaction only specifies the value
        ///  of its outputs. (The value of inputs are recorded
        ///  in the previous transaction outputs being spent.)
        ///  This method therefore raises a 'MissingPreviousOutput'
        ///  error when called on a serialized transaction.
        ///
        ///  If there's no fee set and no change address,
        ///  estimate the fee based on size.
        ///
        ///  *NOTE* : This fee calculation strategy is taken from the MoneyButton/BSV library.
        public Amount GetFee()
        {
            if (IsCoinbase)
            {
                return Amount.Zero;
            }

            if (_fee != Amount.Null) 
            {
                return _fee;
            }

            // if no change output is set, fees should equal all the unspent amount
            return !_hasChangeScript ? GetUnspentValue() : EstimateFee();
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
                var txUpdateOut = new TxOut(txOut.TxHash, txOut.Index, changeAmount, _changeScriptBuilder, true);
                Outputs[(int)txOut.Index] = txUpdateOut;
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
            var inputAmount = InputTotals();
            var outputAmount = NonChangeRecipientTotals();
            var unspent = inputAmount - outputAmount;
            return unspent - GetFee();
        }

        /// Estimates fee from serialized transaction size in bytes.

        
        /// <summary>
        /// Get the transaction unspent value.  
        /// </summary>
        /// <returns></returns>
        private Amount GetUnspentValue() => InputTotals() - RecipientTotals();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Amount EstimateFee()
        {
            var estimatedSize = EstimateSize();
            var available = GetUnspentValue();

            // var fee = BigInt.from((estimatedSize / 1000 * _feePerKb).ceil());
            // if (available > fee)
            // {
            //     estimatedSize += CHANGE_OUTPUT_MAX_SIZE;
            // }
            // fee = BigInt.from((estimatedSize / 1000 * _feePerKb).ceil());
            //
            // return fee;
            
            return Amount.Zero;
        }

        private int EstimateSize()
        {
            // var result = MAXIMUM_EXTRA_SIZE;
            // _txnInputs.forEach((input) {
            //     result += SCRIPT_MAX_SIZE; //TODO: we're only spending P2PKH atm.
            // });
            //
            // _txnOutputs.forEach((output) {
            //     result += HEX
            //         .decode(output.script.toHex())
            //         .length + 9; // <---- HOW DO WE CALCULATE SCRIPT FROM JUST AN ADDRESS !? AND LENGTH ???
            // });
            //
            // return result;

            return 0;
        }

        // void _sortInputs(List<TransactionInput> txns)
        // {
        //     txns.sort((lhs, rhs) {
        //         var txnIdComparison = lhs.prevTxnId.compareTo(rhs.prevTxnId);
        //
        //         if (txnIdComparison != 0)
        //         {
        //             //we use the prevTxnId to sort
        //             return txnIdComparison;
        //         }
        //         else
        //         {
        //             //txnIds can't be used (probably 'cause there's only one)
        //             return lhs.prevTxnOutputIndex - rhs.prevTxnOutputIndex;
        //         }
        //     });
        // }
        //
        // void _sortOutputs(List<TransactionOutput> txns)
        // {
        //     txns.sort((lhs, rhs) {
        //         var satoshiComparison = lhs.satoshis - rhs.satoshis;
        //         if (satoshiComparison != BigInt.zero)
        //         {
        //             return satoshiComparison > BigInt.zero ? 1 : -1;
        //         }
        //         else
        //         {
        //             return lhs.scriptHex.compareTo(rhs.scriptHex);
        //         }
        //     });
        // }

        #endregion
    }
}

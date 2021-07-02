using System;
using System.Linq;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Services;
using CafeLib.BsvSharp.Units;
using CafeLib.Core.Extensions;

namespace CafeLib.BsvSharp.Transactions
{
    public class Transaction : IChainId, IDataSerializer
    {
        private ScriptBuilder _changeScriptBuilder;
        private bool _hasChangeScript = false;
        private Amount _fee = Amount.Null;
        private long _feePerKb = RootService.Network.Consensus.FeePerKilobyte;

        public string TxId => Encoders.HexReverse.Encode(Hash);
        public UInt256 Hash { get; private set; }
        public int Version { get; private set; } = 1;
        public long LockTime { get; private set; }
        public Address ChangeAddress { get; private set; }

        public TxInCollection  Inputs { get; private set; } //this transaction's inputs
        public TxOutCollection Outputs { get; private set; } //this transaction's outputs

        //if we have a Transaction with one input, and a prevTransactionId of zero, it's a coinbase.
        public bool IsCoinbase => Inputs.Count == 1 && Inputs[0].Hash == UInt256.Zero;

        public TransactionOption Option { get; private set; }

        public Transaction()
        {
        }

        public Transaction(int version, TxInCollection vin, TxOutCollection vout, int lockTime, long fee = 0L, TransactionOption option = 0)
        {
            Version = version;
            Inputs = vin;
            Outputs = vout;
            LockTime = lockTime;
            _fee = new Amount(fee);
            Option = option;
        }

        /// <summary>
        /// Add transaction input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Transaction AddInput(TxIn input)
        {
            Inputs.Add(input);
            UpdateChangeOutput();
            return this;
        }

        /// <summary>
        /// Add transaction output/
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public Transaction AddOutput(TxOut output)
        {
            Outputs.Add(output);
            UpdateChangeOutput();
            return this;
        }

        /// <summary>
        /// Add a DataLockBuilder that creates an unspendable, zero-satoshi output
        /// with associated data attached.
        /// 
        /// This method can be called more than once to add multiple data outputs.
        /// 
        /// [data] - The data to add to the output transaction
        /// 
        /// [scriptBuilder] - An instance (or subclass) of [DataLockBuilder] that
        /// will provide the scriptPubKey. The base [DataLockBuilder] will be used
        /// by default, and that results in a very simple data output that has the form
        ///    `OP_FALSE OP_RETURN &lt;data&gt;`
        /// 
        /// Returns an instance of the current Transaction as part of the builder pattern.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scriptBuilder"></param>
        /// <returns></returns>
        public Transaction AddData(byte[] data, ScriptBuilder scriptBuilder = null)
        {
            scriptBuilder ??= new ScriptBuilder();
            scriptBuilder.Add(data);
            var dataOut = new TxOut(Hash, Outputs.Count, scriptBuilder);
            Outputs.Add(dataOut);
            return this;
        }

        /// <summary>
        /// Get change output
        /// </summary>
        /// <param name="changeBuilder"></param>
        /// <returns></returns>
        public TxOut GetChangeOutput(ScriptBuilder changeBuilder)
        {
            var txOut = Outputs.SingleOrDefault(x => x.IsChangeOutput);
            if (txOut != TxOut.Null) return txOut;
            
            txOut = new TxOut(Hash, Outputs.Count, changeBuilder, true);
            Outputs.Add(txOut);
            return txOut;
        }

        /// <summary>
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
        /// </summary>
        /// <returns></returns>
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
            return !_hasChangeScript ? GetUnspentAmount() : EstimateFee();
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
        /// Sort inputs and outputs according to Bip69
        /// </summary>
        /// <returns>transaction</returns>
        public Transaction Sort()
        {
            SortInputs(Inputs);
            SortOutputs(Outputs);
            return this;
        }

        /// <summary>
        /// Add fee to transaction.
        /// </summary>
        /// <param name="fee"></param>
        /// <returns>transaction</returns>
        public Transaction WithFee(Amount fee)
        {
            _fee = fee;
            UpdateChangeOutput();
            return this;
        }

        /// <summary>
        /// With fee per kilobyte.
        /// </summary>
        /// <param name="fee"></param>
        /// <returns></returns>
        public Transaction WithFeePerKilobyte(int fee)
        {
            _feePerKb = fee;
            UpdateChangeOutput();
            return this;
        }

        public IDataWriter WriteTo(IDataWriter writer, object parameters)
        {
            dynamic args = parameters;
            if (args.performChecks)
            {
                DoSerializationChecks();
            }

            return UncheckedSerialize(writer);
        }

        public IDataWriter WriteTo(IDataWriter writer)
        {
            writer
                .Write(Version)
                .Write(Inputs.Count.AsVarIntBytes());

            Inputs.ForEach(x => x.WriteTo(writer));

            writer.Write(Outputs.Count.AsVarIntBytes());

            Outputs.ForEach(x => x.WriteTo(writer));

            writer.Write(LockTime);

            return writer;
        }

        /// <summary>
        /// Set the locktime flag on the transaction to prevent it becoming
        /// spendable before specified date
        ///
        /// [future] - The date in future before which transaction will not be spendable.
        /// </summary>
        /// <param name="future"></param>
        /// <returns></returns>
        public Transaction LockUntilDate(DateTime future)
        {
            if (future.ToUnixTime() < RootService.Network.Consensus.LocktimeBlockheightLimit)
            {
                throw new TransactionException("Block time is set too early");
            }

            Inputs.ForEach(x =>
            {
                if (x.SequenceNumber == RootService.Network.Consensus.DefaultSeqnumber)
                {
                    x.SequenceNumber = (uint) RootService.Network.Consensus.DefaultLocktimeSeqnumber;
                }
            });

            LockTime = future.ToUnixTime();
            return this;
        }



        #region Helpers

        /// <summary>
        ///  Check for missing signature.
        /// </summary>
        /// <exception cref="TransactionException"></exception>
        private void CheckForMissingSignatures() 
        {
            if ((Option & TransactionOption.DisableFullySigned) != 0) return;

            if (!IsFullySigned) 
            {
                throw new TransactionException("Missing Signatures");
            }
        }
        
        /// <summary>
        /// Check for fee errors
        /// </summary>
        /// <param name="unspent">unspent amount</param>
        /// <exception cref="TransactionException"></exception>
        private void CheckForFeeErrors(Amount unspent)
        {
            if (_fee != unspent)
            {
                throw new TransactionException($"Unspent amount is {unspent} but the specified fee is {_fee}.");
            }
            
            if ((Option & TransactionOption.DisableLargeFees) != 0) return;
            
            var maximumFee = RootService.Network.Consensus.FeeSecurityMargin * EstimateFee();
            if (unspent <= maximumFee) return;
            
            if (!_hasChangeScript) 
            {
                throw new TransactionException("Fee is too large and no change address was provided");
            }
            
            throw new TransactionException($"Expected less than {maximumFee} but got {unspent}");
        }

        private bool InputExists(UInt256 txHash, int outputIndex) =>
            Inputs.Any(x => x.PrevOut.TxId == txHash && x.PrevOut.Index == outputIndex);
        
        /// <summary>
        ///  Is the collection of inputs fully signed.
        /// </summary>
        /// <returns></returns>
        private bool IsFullySigned => Inputs.All(x => x.IsFullySigned);
        
        /// <summary>
        /// Update the transaction change output.
        /// </summary>
        private void UpdateChangeOutput()
        {
            if (ChangeAddress == null) return;

            if (_changeScriptBuilder == null) return;

            RemoveChangeOutputs();

            if (NonChangeRecipientTotals() == InputTotals()) return;

            var txOut = GetChangeOutput(_changeScriptBuilder);
            var changeAmount = RecalculateChange();

            ////can't spend negative amount of change :/
            if (changeAmount <= Amount.Zero) return;
            var txUpdateOut = new TxOut(txOut.TxHash, txOut.Index, changeAmount, _changeScriptBuilder, true);
            Outputs[(int)txOut.Index] = txUpdateOut;
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
        /// Get the transaction unspent amount.  
        /// </summary>
        /// <returns></returns>
        private Amount GetUnspentAmount() => InputTotals() - RecipientTotals();

        /// <summary>
        /// Calculate fee estimate.
        /// </summary>
        /// <returns>fee estimate</returns>
        private Amount EstimateFee()
        {
            var estimatedSize = EstimateSize();
            var available = GetUnspentAmount();

            var fee = new Amount((long)Math.Ceiling((double)estimatedSize / 1000 * _feePerKb));
            if (available > fee)
            {
                estimatedSize += RootService.Network.Consensus.ChangeOutputMaxSize;
            }

            fee = new Amount((long)Math.Ceiling((double)estimatedSize / 1000 * _feePerKb));
            return fee;
        }

        /// <summary>
        /// Determine size estimate.
        /// </summary>
        /// <returns></returns>
        private int EstimateSize()
        {
            var result = RootService.Network.Consensus.MaximumExtraSize;
            //_txnInputs.forEach((input) {
            //    result += SCRIPT_MAX_SIZE; //we're only spending P2PKH atm.
            //});
            result += RootService.Network.Consensus.MaximumExtraSize * Inputs.Count;

            // <---- HOW DO WE CALCULATE SCRIPT FROM JUST AN ADDRESS !? AND LENGTH ???
            Outputs.ForEach(x => result += Encoders.Hex.Decode(x.Script.ToHexString()).Length + 9);
            return result;
        }

        /// <summary>
        /// Sort inputs in accordance to BIP69.
        /// </summary>
        /// <param name="inputs"></param>
        private void SortInputs(TxInCollection inputs)
        {
            Inputs = new TxInCollection(inputs.OrderBy(x => x.TxId).ToArray());
        }

        /// <summary>
        /// Sort outputs in accordance to BIP69.
        /// </summary>
        /// <param name="outputs"></param>
        private void SortOutputs(TxOutCollection outputs)
        {
            Outputs = new TxOutCollection(outputs.OrderBy(x => x.Amount).ToArray());
        }

        private void DoSerializationChecks()
        {
            if (Outputs.Any(x => !x.ValidAmount))
            {
                throw new TransactionException("Invalid amount of satoshis");
            }

            var unspent = GetUnspentAmount();

            if (unspent < Amount.Zero)
            {
                if ((Option & TransactionOption.DisableMoreOutputThanInput) == 0)
                {
                    throw new TransactionException("Invalid output sum of satoshis");
                }
            }
            else
            {
                CheckForFeeErrors(unspent);
            }

            CheckForDustErrors();
            CheckForMissingSignatures();
        }

        private void CheckForDustErrors()
        {
            if ((Option & TransactionOption.DisableDustOutputs) != 0) return;

            if (Outputs.Any(x => x.Amount < RootService.Network.Consensus.DustLimit && !x.IsDataOut))
            {
                throw new TransactionException("You have outputs with spending values below the dust limit");
            }
        }

        /// Returns the raw transaction as a hexadecimal string, skipping all checks.
        private IDataWriter UncheckedSerialize(IDataWriter writer)
        {
            // set the transaction version
            writer.Write(Version);

            // set the number of inputs
            writer.Write(new VarInt(Inputs.Count));

            // write the inputs
            Inputs.ForEach(x => x.WriteTo(writer));

            //set the number of outputs to come
            writer.Write(new VarInt(Outputs.Count));

            // write the outputs
            Outputs.ForEach(x => x.WriteTo(writer));

            // write the locktime
            writer.Write(LockTime);

            return writer;

            //return HEX.encode(writer.toBytes().toList());
        }

        #endregion
    }
}

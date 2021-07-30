using System;
using System.Buffers;
using System.Linq;
using System.Security.Cryptography;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Services;
using CafeLib.BsvSharp.Units;
using CafeLib.Core.Buffers;
using CafeLib.Core.Extensions;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    /// 
    /// </summary>
    public class TxBuilder
    {
        private int _version;
        private uint _lockTime;
        private TxInCollection _inputs;
        private TxOutCollection _outputs;
        private UInt256 _txHash;
        private ScriptBuilder _changeScriptBuilder;
        private bool _hasChangeScript;
        private Amount _fee = Amount.Null;
        private long _feePerKb = RootService.Network.Consensus.FeePerKilobyte;
        private Address _changeAddress;
        private TransactionOption _option;
        

        // private TxOutMap TxOutMap { get; set; }
        // private SigOperations SigOps { get; set; }
        // private Script ChangeScript { get; set; }
        // private Amount ChangeAmount { get; set; }
        // private Amount FeePerKbAmount { get; set; }
        // private uint LockTime { get; set; }
        // private uint Version { get; set; }
        // private uint SigsPerInput { get; set; }
        // private Amount Dust { get; set; }
        // private bool SendDustChangeToFees { get; set; }
        // private HashCache HashCache { get; set; }

        public TxBuilder()
        {
            _version = 1;
            _lockTime = 0;
            _fee = new Amount();
            _inputs = new TxInCollection();
            _outputs = new TxOutCollection();
        }

        /// <summary>
        /// Transaction constructor. 
        /// </summary>
        /// <param name="tx">transaction</param>
        public TxBuilder(Transaction tx)
            : this()
        {
            _version = tx.Version;
            _lockTime = tx.LockTime;
            _inputs.AddRange(tx.Inputs);
            _outputs.AddRange(tx.Outputs);
            _txHash = tx.Hash;
        }
        
        public TxBuilder(string hex)
            : this(Encoders.Hex.Decode(hex))
        {
        }
        
        public TxBuilder(byte[] bytes)
        {
            var reader = new ByteSequenceReader(bytes);
            TryReadTransaction(ref reader);
        }
        
        /// <summary>
        /// IsCoinbase
        /// </summary>
        public bool IsCoinbase => _inputs.Count == 1 && _inputs[0].Hash == UInt256.Zero;
        
        
        /// <summary>
        /// Add transaction input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Transaction AddInput(TxIn input)
        {
            _inputs.Add(input);
            UpdateChangeOutput();
            return this;
        }

        /// <summary>
        /// Add transaction output/
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public TxBuilder AddOutput(TxOut output)
        {
            _outputs.Add(output);
            UpdateChangeOutput();
            return this;
        }
        
        /// <summary>
        /// Get change output
        /// </summary>
        /// <param name="changeBuilder"></param>
        /// <returns></returns>
        public TxOut GetChangeOutput(ScriptBuilder changeBuilder)
        {
            var txOut = _outputs.SingleOrDefault(x => x.IsChangeOutput);
            if (txOut != null) return txOut;

            txOut = new TxOut(_txHash, _outputs.Count, changeBuilder, true);
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
        public TxBuilder SendChangeTo(Address changeAddress, ScriptBuilder scriptBuilder = null)
        {
            scriptBuilder ??= new P2PkhScriptBuilder(changeAddress);

            _hasChangeScript = true;
            //get fee, and if there is not enough change to cover fee, remove change outputs

            //delete previous change transaction if exists
            _changeAddress = changeAddress;
            _changeScriptBuilder = scriptBuilder;
            UpdateChangeOutput();
            return this;
        }

        /// <summary>
        /// Spend from Utxo
        /// </summary>
        /// <param name="txHash">utxo transaction hash</param>
        /// <param name="outputIndex">utxo index</param>
        /// <param name="amount">amount</param>
        /// <param name="scriptPubKey">script pub key</param>
        /// <returns>transaction</returns>
        public Transaction SpendFrom(UInt256 txHash, int outputIndex, Amount amount, Script scriptPubKey)
        {
            var txIn = new TxIn(txHash, outputIndex, amount, scriptPubKey);
            _inputs.Add(txIn);
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

            scriptBuilder ??= new P2PkhLockBuilder(recipient);
            var txOut = new TxOut(_txHash, _outputs.Count, sats, scriptBuilder);
            return AddOutput(txOut);
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
        public Transaction WithFeePerKb(int fee)
        {
            _feePerKb = fee;
            UpdateChangeOutput();
            return this;
        }
        
        
        /// <summary>
        ///     Import a transaction partially signed by someone else. The only thing you
        ///     can do after this is sign one or more inputs. Usually used for multisig
        ///     transactions. uTxOutMap is optional. It is not necessary so long as you
        ///     pass in the txOut when you sign. You need to know the output when signing
        ///     an input, including the script in the output, which is why this is
        ///     necessary when signing an input.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="txOutMap"></param>
        /// <param name="signOperations"></param>
        /// <returns></returns>
        // public TxBuilder ImportPartiallySignedTx(Chain.Transaction tx, TxOutMap txOutMap = null, SigOperations signOperations = null)
        // {
        //     Tx = tx ?? Tx;
        //     TxOutMap = txOutMap ?? TxOutMap;
        //     SigOps = signOperations ?? SigOps;
        //     return this;
        // }

        // public TxBuilder InputFromScript(UInt256 txHashBuf, int txOutIndex, TxOut txOut, Script script, uint sequence)
        // {
        //     Vin.Add(new TxIn(new OutPoint(txHashBuf, txOutIndex), Amount.Zero, script, sequence));
        //     TxOutMap.Set(txHashBuf, txOutIndex, txOut);
        //     return this;
        // }
        
        /// <summary>
        /// Deserialize transaction.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool TryReadTransaction(ref ByteSequenceReader r)
        {
            var start = r.Data.Position;

            if (!r.TryReadLittleEndian(out int _version)) return false;

            if (!r.TryReadVariant(out var countIn)) return false;
            _inputs = new TxInCollection();
            for (var i = 0L; i < countIn; i++)
            {
                var txIn = new TxIn();
                if (!txIn.TryReadTxIn(ref r)) return false;
                _inputs.Add(txIn);
            }

            if (!r.TryReadVariant(out var countOut)) return false;
            _outputs = new TxOutCollection();
            for (var i = 0L; i < countOut; i++)
            {
                var txOut = new TxOut();
                if (!txOut.TryReadTxOut(ref r)) return false;
                _outputs.Add(txOut);
            }

            if (!r.TryReadLittleEndian(out uint _lockTime)) return false;

            var end = r.Data.Position;

            // Compute the transaction hash.
            var txBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using var sha256 = SHA256.Create();
            var hash1 = sha256.ComputeHash(txBytes);
            var hash2 = sha256.ComputeHash(hash1);
            _txHash = new UInt256(hash2);
            return true;
        }

        public Transaction Build()
        {
            return new Transaction();
        }
        
        public static implicit operator Transaction(TxBuilder rhs) => rhs.Build();
        public static explicit operator TxBuilder(Transaction rhs) => new TxBuilder(rhs);
        
        #region Helpers

        /// <summary>
        ///  Check for missing signature.
        /// </summary>
        /// <exception cref="TransactionException"></exception>
        private void CheckForMissingSignatures()
        {
            if ((_option & TransactionOption.DisableFullySigned) != 0) return;

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

            if ((_option & TransactionOption.DisableLargeFees) != 0) return;

            var maximumFee = RootService.Network.Consensus.FeeSecurityMargin * EstimateFee();
            if (unspent <= maximumFee) return;

            if (!_hasChangeScript)
            {
                throw new TransactionException("Fee is too large and no change address was provided");
            }

            throw new TransactionException($"Expected less than {maximumFee} but got {unspent}");
        }

        /// <summary>
        /// Determines whether an input transaction exist. 
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="outputIndex"></param>
        /// <returns></returns>
        private bool InputExists(UInt256 txHash, int outputIndex) =>
            _inputs.Any(x => x.PrevOut.TxId == txHash && x.PrevOut.Index == outputIndex);

        /// <summary>
        ///  Is the collection of inputs fully signed.
        /// </summary>
        /// <returns></returns>
        private bool IsFullySigned => _inputs.All(x => x.IsFullySigned);

        /// <summary>
        /// Update the transaction change output.
        /// </summary>
        private void UpdateChangeOutput()
        {
            if (_changeAddress == null) return;

            if (_changeScriptBuilder == null) return;

            RemoveChangeOutputs();

            if (NonChangeRecipientTotals() == InputTotals()) return;

            var txOut = GetChangeOutput(_changeScriptBuilder);
            var changeAmount = RecalculateChange();

            ////can't spend negative amount of change :/
            if (changeAmount <= Amount.Zero) return;
            _outputs.Add(new TxOut(txOut.TxHash, 0, changeAmount, _changeScriptBuilder, true));
        }

        private void RemoveChangeOutputs() => _outputs.Where(x => x.IsChangeOutput).ForEach(x => _outputs.Remove(x));

        private Amount NonChangeRecipientTotals() =>
            _outputs
                .Where(txOut => !txOut.IsChangeOutput)
                .Aggregate(Amount.Zero, (prev, x) => prev + x.Amount);

        private Amount RecipientTotals() => _outputs.Aggregate(Amount.Zero, (prev, x) => prev + x.Amount);

        private Amount InputTotals() => _inputs.Aggregate(Amount.Zero, (prev, x) => prev + x.Amount);

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
            //    result += SCRIPT_MAX_SIZE; 
            //});

            //Note: we're only spending P2PKH atm.
            result += RootService.Network.Consensus.ScriptMaxSize * _inputs.Count;

            // <---- HOW DO WE CALCULATE SCRIPT FROM JUST AN ADDRESS !? AND LENGTH ???
            _outputs.ForEach(x => result += Encoders.Hex.Decode(x.Script.ToHexString()).Length + 9);
            return result;
        }

        /// <summary>
        /// Sort inputs in accordance to BIP69.
        /// </summary>
        /// <param name="inputs"></param>
        private void SortInputs(TxInCollection inputs)
        {
            _inputs = new TxInCollection(inputs.OrderBy(x => x.TxId).ToArray());
        }

        /// <summary>
        /// Sort outputs in accordance to BIP69.
        /// </summary>
        /// <param name="outputs"></param>
        private void SortOutputs(TxOutCollection outputs)
        {
            _outputs = new TxOutCollection(outputs.OrderBy(x => x.Amount).ToArray());
        }

        private void DoSerializationChecks()
        {
            if (_outputs.Any(x => !x.ValidAmount))
            {
                throw new TransactionException("Invalid amount of satoshis");
            }

            var unspent = GetUnspentAmount();

            if (unspent < Amount.Zero)
            {
                if ((_option & TransactionOption.DisableMoreOutputThanInput) == 0)
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
            if ((_option & TransactionOption.DisableDustOutputs) != 0) return;

            if (_outputs.Any(x => x.Amount < RootService.Network.Consensus.DustLimit && !x.IsDataOut))
            {
                throw new TransactionException("You have outputs with spending values below the dust limit");
            }
        }

        //private void Sign(TransactionInput input, SVPrivateKey privateKey, { sighashType = SighashType.SIGHASH_ALL | SighashType.SIGHASH_FORKID}){

        //    //FIXME: This is a test work-around for why I can't sign an unsigned raw txn
        //    //FIXME: This assumes we're signing P2PKH

        //    //FIXME: This should account for ANYONECANPAY mask that limits outputs to sign over
        //    ///      NOTE: Stripping Subscript should be done inside SIGHASH class
        //    var subscript = input.subScript; //scriptSig FIXME: WTF !? Sighash should fail on this
        //    var inputIndex = inputs.indexOf(input);
        //    var sigHash = Sighash();
        //    var hash = sigHash.hash(this, sighashType, inputIndex, subscript, input.satoshis);

        //    //FIXME: Revisit this issue surrounding the need to sign a reversed copy of the hash.
        //    ///      Right now I've factored this out of signature.dart because 'coupling' & 'separation of concerns'.
        //    var reversedHash = HEX.encode(HEX
        //        .decode(hash)
        //        .reversed
        //        .toList());

        //    // generate a signature for the input
        //    var sig = SVSignature.fromPrivateKey(privateKey);
        //    sig.nhashtype = sighashType;
        //    sig.sign(reversedHash);

        //    if (input.scriptBuilder is SignedUnlockBuilder) {

        //        //culminate in injecting the derived signature into the ScriptBuilder instance
        //        (input.scriptBuilder as SignedUnlockBuilder).signatures.add(sig);
        //    }else{
        //        throw TransactionException("Trying to sign a Transaction Input that is missing a SignedUnlockBuilder");
        //    }

        //}

        #endregion
    }
}

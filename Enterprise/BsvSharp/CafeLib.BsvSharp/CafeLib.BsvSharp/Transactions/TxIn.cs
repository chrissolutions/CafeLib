﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Exceptions;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Signatures;
using CafeLib.BsvSharp.Units;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction input as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// Not used for making dynamic changes (building scripts).
    /// See <see cref="Transaction"/> when dynamically building a transaction input.
    /// </summary>
    public class TxIn : ITxId, IDataSerializer
    {
        /// <summary>
        /// This is the ScriptPub of the referenced Prevout.
        /// Used to sign and verify this input.
        /// </summary>
        private readonly ScriptBuilder _scriptBuilder;

        /// <summary>
        /// Setting nSequence to this value for every input in a transaction disables nLockTime.
        /// </summary>
        public const uint SequenceFinal = 0xffffffff;

        /// <summary>
        /// Below flags apply in the context of Bip 68.
        /// If this flag set, txIn.nSequence is NOT interpreted as a relative lock-time.
        /// </summary>
        public const uint SequenceLocktimeDisableFlag = 1U << 31;

        /// <summary>
        /// If txIn.nSequence encodes a relative lock-time and this flag is set, the relative lock-time
        /// has units of 512 seconds, otherwise it specifies blocks with a granularity of 1.
        /// </summary>
        public const int SequenceLocktimeTypeFlag = 1 << 22;

        /// <summary>
        /// If txIn.nSequence encodes a relative lock-time, this mask is applied to extract that lock-time
        /// from the sequence field.
        /// </summary>
        public const uint SequenceLocktimeMask = 0x0000ffff;

        /* In order to use the same number of bits to encode roughly the same
           * wall-clock duration, and because blocks are naturally limited to occur
           * every 600s on average, the minimum granularity for time-based relative
           * lock-time is fixed at 512 seconds.  Converting from CTxIn::nSequence to
           * seconds is performed by multiplying by 512 = 2^9, or equivalently
           * shifting up by 9 bits. */
        public const uint SequenceLocktimeGranularity = 9;

        public UInt256 TxHash => PrevOut.TxId;

        public string TxId => Encoders.HexReverse.Encode(TxHash);
        public int Index => PrevOut.Index;

        public OutPoint PrevOut { get; private set; }

        public Script UtxoScript { get; private set; }

        public uint SequenceNumber { get; set; }

        public Amount Amount { get; private set; }

        /// <summary>
        /// This is used by the Transaction during serialization checks.
        /// It is only used in the context of P2PKH transaction types and
        /// will likely be deprecated in future.
        /// 
        /// FIXME: Perform stronger check than this. We should be able to
        /// validate the _scriptBuilder Signatures. At the moment this is more
        /// of a check on where a signature is required.
        /// </summary>
        public bool IsFullySigned { get; }

        /// <summary>
        /// TxIn default constructor.
        /// </summary>
        public TxIn()
        {
        }

        /// <summary>
        /// Transaction input constructor.
        /// </summary>
        /// <param name="prevOutPoint"></param>
        /// <param name="amount"></param>
        /// <param name="utxoScript"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="scriptBuilder"></param>
        public TxIn(OutPoint prevOutPoint, Amount amount, Script utxoScript, uint sequenceNumber, ScriptBuilder scriptBuilder = null)
        {
            PrevOut = prevOutPoint;
            Amount = amount;
            UtxoScript = utxoScript;
            _scriptBuilder = scriptBuilder ?? new SignedUnlockBuilder();
            SequenceNumber = sequenceNumber;
            IsFullySigned = _scriptBuilder is SignedUnlockBuilder;
        }

        /// <summary>
        /// Transaction input constructor.
        /// </summary>
        /// <param name="prevTxId"></param>
        /// <param name="outIndex"></param>
        /// <param name="amount"></param>
        /// <param name="utxoScript"></param>
        /// <param name="nSequenceIn"></param>
        /// <param name="scriptBuilder"></param>
        public TxIn(UInt256 prevTxId, int outIndex, Amount amount, Script utxoScript = new Script(), uint nSequenceIn = SequenceFinal, ScriptBuilder scriptBuilder = null)
            : this(new OutPoint(prevTxId, outIndex), amount, utxoScript, nSequenceIn, scriptBuilder)
        {
        }

        public IDataWriter WriteTo(IDataWriter writer, object parameters) => WriteTo(writer);
        public IDataWriter WriteTo(IDataWriter writer)
        {
            writer.Write(Encoders.HexReverse.Decode(TxId));
            writer.Write(Index);
            writer.Write(UtxoScript);
            writer.Write(SequenceNumber);
            return writer;
        }

        internal bool Sign(Transaction tx, PrivateKey privateKey, SignatureHashEnum sighashType = SignatureHashEnum.All | SignatureHashEnum.ForkId)
            => Sign(tx, privateKey, false, sighashType);

        internal bool Sign(Transaction tx, PrivateKey privateKey, bool confirmExistingSignatures, SignatureHashEnum sighashType = SignatureHashEnum.All | SignatureHashEnum.ForkId)
        {
            var signedOk = true;
            var sigHash = new SignatureHashType(SignatureHashEnum.All | SignatureHashEnum.ForkId);
            var scriptSig = new ScriptBuilder(UtxoScript);

            if (scriptSig.Ops.Count == 2)
            {
                var publicKey = new PublicKey();
                publicKey.Set(scriptSig.Ops[1].Operand.Data);
                if (privateKey == null || !publicKey.IsValid) return false;

                Amount = Amount >= Amount.Zero 
                    ? Amount 
                    : tx.Outputs[PrevOut.Index].Amount >= Amount.Zero 
                        ? tx.Outputs[PrevOut.Index].Amount
                        : Amount.Zero;

                var signatureHash = TransactionSignatureChecker.ComputeSignatureHash(_scriptBuilder, tx, tx.Inputs.IndexOf(this), sigHash, Amount);
                var signature = privateKey.CreateSignature(signatureHash);
                if (signature == null) return false;
    
                var sigWithType = new byte[signature.Length + 1];
                signature.CopyTo(sigWithType.AsSpan());
                sigWithType[^1] = (byte)sigHash.RawSigHashType;
                var op = Operand.Push(sigWithType.AsSpan());
                if (confirmExistingSignatures)
                    signedOk &= op == scriptSig.Ops[0].Operand;
                else
                    scriptSig.Ops[0] = op;
            }

            return signedOk;
        }

        internal void Sign2(Transaction tx, PrivateKey privateKey, SignatureHashEnum sighashType = SignatureHashEnum.All | SignatureHashEnum.ForkId)
        {
            var sigHash = new SignatureHashType(SignatureHashEnum.All | SignatureHashEnum.ForkId);
            var signatureHash = TransactionSignatureChecker.ComputeSignatureHash(_scriptBuilder, tx, tx.Inputs.IndexOf(this), sigHash, Amount);
            var signature = new Signature(privateKey.CreateSignature(signatureHash));

            if (_scriptBuilder is SignedUnlockBuilder builder)
            {
                //culminate in injecting the derived signature into the ScriptBuilder instance
                builder.AddSignature(signature);
            }
            else
            {
                throw new TransactionException("Trying to sign a Transaction Input that is missing a SignedUnlockBuilder");
            }
        }

        public bool TryReadTxIn(ref ByteSequenceReader r)
        {
            var prevOut = new OutPoint();
            if (!prevOut.TryReadOutPoint(ref r)) return false;
            PrevOut = prevOut;

            var script = new Script();
            if (!script.TryReadScript(ref r)) return false;
            UtxoScript = script;

            if (!r.TryReadLittleEndian(out uint sequenceNumber)) return false;
            SequenceNumber = sequenceNumber;
            return true;
        }
    }
}

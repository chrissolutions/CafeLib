﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction input as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// Not used for making dynamic changes (building scripts).
    /// See <see cref="Chain.Transaction"/> when dynamically building a transaction input.
    /// <seealso cref="TxInBuilder"/>
    /// </summary>
    public struct TxIn : IChainId
    {
        private OutPoint _prevOutPoint;
        private Script _scriptSig;
        private uint _sequenceNumber;
        private Amount _spendingAmount;
        private bool _isSignedInput;
        private ScriptBuilder _scriptBuilder;

        //private Script _utxoScript;


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

        public UInt256 Hash => PrevOut.TxId;

        public string TxId => Encoders.HexReverse.Encode(Hash);
        public int Index => PrevOut.Index;

        public OutPoint PrevOut => _prevOutPoint;
        public Script ScriptSig => _scriptSig;
        public uint Sequence => _sequenceNumber;
        public Amount Amount => _spendingAmount;

        /// <summary>
        /// This is used by the Transaction during serialization checks.
        /// It is only used in the context of P2PKH transaction types and
        /// will likely be deprecated in future.
        /// 
        /// FIXME: Perform stronger check than this. We should be able to
        /// validate the _scriptBuilder Signatures. At the moment this is more
        /// of a check on where a signature is required.
        /// </summary>
        public bool IsFullySigned => _isSignedInput;

        /// <summary>
        /// Transaction input constructor.
        /// </summary>
        /// <param name="prevOutPoint"></param>
        /// <param name="amount"></param>
        /// <param name="scriptSig"></param>
        /// <param name="sequence"></param>
        /// <param name="scriptBuilder"></param>
        public TxIn(OutPoint prevOutPoint, Amount amount, Script scriptSig, uint sequence, ScriptBuilder scriptBuilder = null)
        {
            _prevOutPoint = prevOutPoint;
            _spendingAmount = amount;
            _scriptSig = scriptSig;
            _sequenceNumber = sequence;
            _isSignedInput = false;
            _scriptBuilder = scriptBuilder;
        }

        /// <summary>
        /// Transaction input constructor.
        /// </summary>
        /// <param name="prevTxId"></param>
        /// <param name="outIndex"></param>
        /// <param name="amount"></param>
        /// <param name="scriptSigIn"></param>
        /// <param name="nSequenceIn"></param>
        /// <param name="scriptBuilder"></param>
        public TxIn(UInt256 prevTxId, int outIndex, Amount amount, Script scriptSigIn = new Script(), uint nSequenceIn = SequenceFinal, ScriptBuilder scriptBuilder = null)
            : this(new OutPoint(prevTxId, outIndex), amount, scriptSigIn, nSequenceIn, scriptBuilder)
        {
        }

        public byte[] Serialize()
        {
            var writer = new ByteDataWriter();

            writer.Write(Encoders.HexReverse.Decode(TxId));
            writer.Write(Index);

            //var scriptHex = HEX.decode(_scriptBuilder.getScriptSig().toHex());
            //writer.write(varIntWriter(scriptHex.length).toList(), copy: true);
            //writer.write(scriptHex, copy: true);

            writer.Write(_sequenceNumber);
            return writer.ToArray();
        }


        public bool TryParseTxIn(ref ByteSequenceReader r, IBlockParser bp)
        {
            if (!_prevOutPoint.TryReadOutPoint(ref r)) goto fail;

            //bp.TxInStart(this, r.Data.Consumed);

            if (!_scriptSig.TryParseScript(ref r, bp)) goto fail;
            if (!r.TryReadLittleEndian(out _sequenceNumber)) goto fail;

            //bp.TxInParsed(this, r.Data.Consumed);

            return true;
            fail:
            return false;
        }

        public bool TryReadTxIn(ref ByteSequenceReader r)
        {
            if (!_prevOutPoint.TryReadOutPoint(ref r)) goto fail;
            if (!_scriptSig.TryReadScript(ref r)) goto fail;
            if (!r.TryReadLittleEndian(out _sequenceNumber)) goto fail;

            return true;
            fail:
            return false;
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer
                .Add(_prevOutPoint)
                .Add(_scriptSig)
                .Add(_sequenceNumber);
            return writer;
        }
    }
}
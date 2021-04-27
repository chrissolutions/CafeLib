#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Builders;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;
using CafeLib.Bitcoin.Scripting;

namespace CafeLib.Bitcoin.Chain 
{
    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction input as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// Not used for making dynamic changes (building scripts).
    /// See <see cref="TxInBuilder"/> when dynamically building a transaction input.
    /// <seealso cref="Transaction"/>
    /// </summary>
    public struct TxIn
    {
        private OutPoint _prevOutPoint;
        private Script _scriptSig;
        private uint _sequence;

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

        public UInt256 TxId => PrevOut.TxId;
        public OutPoint PrevOut => _prevOutPoint;
        public Script ScriptSig => _scriptSig;
        public uint Sequence => _sequence;

        public TxIn(OutPoint prevOutPoint, Script scriptSig, uint sequence)
        {
            _prevOutPoint = prevOutPoint;
            _scriptSig = scriptSig;
            _sequence = sequence;
        }

        public TxIn(UInt256 prevTxId, int outIndex, Script scriptSigIn = new Script(), uint nSequenceIn = SequenceFinal)
            : this(new OutPoint(prevTxId, outIndex), scriptSigIn, nSequenceIn)
        {
        }

        public bool TryParseTxIn(ref ByteSequenceReader r, IBlockParser bp)
        {
            if (!_prevOutPoint.TryReadOutPoint(ref r)) goto fail;

            bp.TxInStart(this, r.Data.Consumed);

            if (!_scriptSig.TryParseScript(ref r, bp)) goto fail;
            if (!r.TryReadLittleEndian(out _sequence)) goto fail;

            bp.TxInParsed(this, r.Data.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadTxIn(ref ByteSequenceReader r)
        {
            if (!_prevOutPoint.TryReadOutPoint(ref r)) goto fail;
            if (!_scriptSig.TryReadScript(ref r)) goto fail;
            if (!r.TryReadLittleEndian(out _sequence)) goto fail;

            return true;
        fail:
            return false;
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer
                .Add(_prevOutPoint)
                .Add(_scriptSig)
                .Add(_sequence);
            return writer;
        }
    }
}

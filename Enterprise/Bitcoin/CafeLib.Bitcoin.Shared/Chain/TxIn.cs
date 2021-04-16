#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Persistence;
using CafeLib.Bitcoin.Shared.Scripting;

namespace CafeLib.Bitcoin.Shared.Chain 
{
    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction input as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// Not used for making dynamic changes (building scripts).
    /// See <see cref="KzBTxIn"/> when dynamically building a transaction input.
    /// <seealso cref="Transaction"/>
    /// </summary>
    public struct TxIn
    {
        /// <summary>
        /// Setting nSequence to this value for every input in a transaction disables nLockTime.
        /// </summary>
        public const UInt32 SequenceFinal = 0xffff_ffff;

        OutPoint _prevOutPoint;
        Script _scriptSig;
        UInt32 _sequence;

        public OutPoint PrevOut => _prevOutPoint;
        public Script ScriptSig => _scriptSig;
        public UInt32 Sequence => _sequence;

        public TxIn(OutPoint prevOutPoint, Script scriptSig, UInt32 sequence)
        {
            _prevOutPoint = prevOutPoint;
            _scriptSig = scriptSig;
            _sequence = sequence;
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

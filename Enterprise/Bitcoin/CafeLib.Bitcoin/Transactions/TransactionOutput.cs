#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.IO;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;
using CafeLib.Bitcoin.Scripting;
using CafeLib.Bitcoin.Units;

namespace CafeLib.Bitcoin.Transactions
{
    public struct TransactionOutput
    {
        private UInt256 _txHash;
        private long _index;
        private Amount _amount;
        private Script _script;
        private bool _isChangeOutput;

        public UInt256 TxHash => _txHash;
        public long Index => _index;
        private Amount Amount => _amount;
        private bool IsChangeOutput => _isChangeOutput;
        private Script Script => _script;

        /// <summary>
        /// Null transaction output
        /// </summary>
        public static TransactionOutput Null => new TransactionOutput { _txHash = UInt256.Zero, _index = -1, _amount = Amount.Null, _script = Script.None};

        public TransactionOutput(UInt256 txHash, long index, Amount amount, Script script, bool isChangeOutput = false)
        {
            _txHash = txHash;
            _index = index;
            _amount = amount;
            _script = script;
            _isChangeOutput = isChangeOutput;
        }

        //public bool TryParseTxOut(ref ByteSequenceReader r, IBlockParser bp)
        //{
        //    if (!r.TryReadLittleEndian(out _amount)) goto fail;

        //    bp.TxOutStart(this, r.Data.Consumed);

        //    if (!_script.TryParseScript(ref r, bp)) goto fail;

        //    bp.TxOutParsed(this, r.Data.Consumed);

        //    return true;
        //fail:
        //    return false;
        //}

        //public bool TryReadTxOut(ref ByteSequenceReader reader)
        //{
        //    if (!reader.TryReadLittleEndian(out _amount)) goto fail;
        //    if (!_script.TryReadScript(ref reader)) goto fail;

        //    return true;
        //fail:
        //    return false;
        //}

        //public void Write(BinaryWriter s)
        //{
        //    s.Write(_amount);
        //    _script.Write(s);
        //}

        //public void Read(BinaryReader s)
        //{
        //    _amount = s.ReadInt64();
        //    _script.Read(s);
        //}

        //public override string ToString()
        //{
        //    return $"{new Amount(_amount)} {_script}";
        //}

        //public IBitcoinWriter AddTo(IBitcoinWriter writer)
        //{
        //    writer
        //        .Add(_amount)
        //        .Add(_script)
        //        ;
        //    return writer;
        //}
    }
}

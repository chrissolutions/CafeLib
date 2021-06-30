#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Transactions
{
    public struct TxOut : IChainId
    {
        private UInt256 _txHash;
        private long _index;
        private Amount _amount;
        private ScriptBuilder _scriptBuilder;
        private bool _isChangeOutput;

        public UInt256 TxHash => _txHash;
        public long Index => _index;
        private Amount Amount => _amount;
        private bool IsChangeOutput => _isChangeOutput;
        private Script Script => _scriptBuilder;

        public UInt256 Hash => _txHash;


        /// <summary>
        /// Null transaction output
        /// </summary>
        public static TxOut Null => new TxOut { _txHash = UInt256.Zero, _index = -1, _amount = Amount.Null, _scriptBuilder = null};

        public TxOut(UInt256 txHash, long index, Amount amount, ScriptBuilder script, bool isChangeOutput = false)
        {
            _txHash = txHash;
            _index = index;
            _amount = amount;
            _scriptBuilder = script;
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

        public override string ToString()
        {
            return $"{new Amount(_amount)} {_scriptBuilder.ToScript()}";
        }

        //public IBitcoinWriter AddTo(IBitcoinWriter writer)
        //{
        //    writer
        //        .Add(_amount)
        //        .Add(_script)
        //        ;
        //    return writer;
        //}

        #region Helpers

        /// <summary>
        ///Returns true is satoshi amount if outside of valid range
        /// See [Transaction.MAX_MONEY]
        /// </summary>
        /// <returns></returns>
        private bool InvalidSatoshis() => _amount.Satoshis < Amount.Zero || _amount.Satoshis > Amount.MaxValue;

        #endregion

    }
}

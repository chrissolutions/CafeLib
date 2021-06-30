#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Linq;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Transactions
{
    public struct TxOut : IChainId
    {
        private ScriptBuilder _scriptBuilder;

        public UInt256 TxHash { get; private set; }
        public long Index { get; private set; }

        public Amount Amount { get; set; }
        public bool IsChangeOutput { get; set; }

        private Script Script => _scriptBuilder;

        public UInt256 Hash => TxHash;

        /// <summary>
        /// Convenience property to check if this output has been made unspendable
        /// using either an OP_RETURN or "OP_FALSE OP_RETURN" in first positions of
        /// the script.
        /// </summary>
        /// <returns></returns>
        public bool IsDataOut => _scriptBuilder.Ops.Any() && _scriptBuilder.Ops[0].Operand.Code == Opcode.OP_FALSE
                   || _scriptBuilder.Ops.Count >= 2 && _scriptBuilder.Ops[0].Operand.Code == Opcode.OP_RETURN;

        /// <summary>
        /// Null transaction output
        /// </summary>
        public static TxOut Null => new TxOut { TxHash = UInt256.Zero, Index = -1, Amount = Amount.Null, _scriptBuilder = null};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        /// <param name="script"></param>
        /// <param name="isChangeOutput"></param>
        public TxOut(UInt256 txHash, long index, ScriptBuilder script, bool isChangeOutput = false)
            : this (txHash, index, Amount.Zero, script, isChangeOutput)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        /// <param name="script"></param>
        /// <param name="isChangeOutput"></param>
        public TxOut(UInt256 txHash, long index, Amount amount, ScriptBuilder script, bool isChangeOutput = false)
        {
            TxHash = txHash;
            Index = index;
            Amount = amount;
            _scriptBuilder = script;
            IsChangeOutput = isChangeOutput;
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
            return $"{new Amount(Amount)} {_scriptBuilder.ToScript()}";
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
        private bool InvalidSatoshis() => Amount.Satoshis < Amount.Zero || Amount.Satoshis > Amount.MaxValue;

        #endregion

    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Transactions
{
    public struct TxOut : IChainId, IDataSerializer, IEquatable<TxOut>
    {
        private ScriptBuilder _scriptBuilder;

        public UInt256 TxHash { get; }
        public long Index { get; }

        public Amount Amount { get; private set; }
        public bool IsChangeOutput { get; }

        public Script Script => _scriptBuilder;

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
        public static TxOut Null => new TxOut(UInt256.Zero, -1, Amount.Null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="index"></param>
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

        public bool TryReadTxOut(ref ByteSequenceReader reader)
        {
            if (!reader.TryReadLittleEndian(out long amount)) return false;
            Amount = amount;

            var script = new Script();
            if (!script.TryReadScript(ref reader)) return false;
            _scriptBuilder = new ScriptBuilder(script);
            return true;
        }

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

        /// <summary>
        /// Returns true is satoshi amount is within valid range
        /// </summary>
        public bool ValidAmount => Amount >= Amount.Zero && Amount <= Amount.MaxValue;

        /// <summary>
        /// Returns string representation of TxOut.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{new Amount(Amount)} {_scriptBuilder.ToScript()}";

        /// <summary>
        /// Write TxOut to data writer
        /// </summary>
        /// <param name="writer">data writer</param>
        /// <param name="parameters">parameters</param>
        /// <returns>data writer</returns>
        public IDataWriter WriteTo(IDataWriter writer, object parameters) => WriteTo(writer);
        
        /// <summary>
        /// Write TxOut to data writer
        /// </summary>
        /// <param name="writer">data writer</param>
        /// <returns>data writer</returns>
        public IDataWriter WriteTo(IDataWriter writer)
        {
            writer
                .Write(Amount)
                .Write(_scriptBuilder.ToScript())
                ;
            return writer;
        }

        //public IBitcoinWriter AddTo(IBitcoinWriter writer)
        //{
        //    writer
        //        .Add(_amount)
        //        .Add(_script)
        //        ;
        //    return writer;
        //}

        public override int GetHashCode() => HashCode.Combine(_scriptBuilder, TxHash, Index, IsChangeOutput);

        public bool Equals(TxOut other)
        {
            return Equals(_scriptBuilder, other._scriptBuilder) && TxHash.Equals(other.TxHash) && Index == other.Index && Amount.Equals(other.Amount) && IsChangeOutput == other.IsChangeOutput;
        }

        public override bool Equals(object obj)
        {
            return obj is TxOut other && Equals(other);
        }

        public static bool operator ==(TxOut x, TxOut y) => x.Equals(y);
        public static bool operator !=(TxOut x, TxOut y) => !(x == y);

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

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
// ReSharper disable ConvertToAutoProperty

namespace CafeLib.Bitcoin.Transactions
{
    public struct Utxo
    {
        private UInt256 _txHash;
        private long _index;
        private Amount _amount;
        private int _height;
        private bool _isCoinbase;
        private Script _script;
        private string _address;


        public UInt256 TxHash => _txHash;
        public long Index => _index;
        private Amount Amount => _amount;
        private int Height => _height;
        private bool IsCoinbase => _isCoinbase;
        private Script Script => _script;
        private string Address => _address;

        /// <summary>
        /// Creates a stored transaction output.
        /// </summary>
        /// <param name="txHash">   The hash of the containing transaction. </param>
        /// <param name="index">    The outpoint. </param>
        /// <param name="amount">   The amount available. </param>
        /// <param name="height">   The height this output was created in. </param>
        /// <param name="coinbase"> The coinbase flag. </param>
        /// <param name="script">   The output script</param>
        public Utxo(UInt256 txHash, long index, Amount amount, int height, bool coinbase, Script script)
        {
            _txHash = txHash;
            _index = index;
            _amount = amount;
            _height = height;
            _script = script;
            _isCoinbase = coinbase;
            _address = "";
        }

        /// <summary>
        /// Creates a stored transaction output.
        /// </summary>
        /// <param name="txHash">   The hash of the containing transaction. </param>
        /// <param name="index">    The outpoint. </param>
        /// <param name="amount">   The amount available. </param>
        /// <param name="height">   The height this output was created in. </param>
        /// <param name="coinbase"> The coinbase flag. </param>
        /// <param name="script">   The output script</param>
        /// <param name="address">  The output address</param>
        public Utxo(UInt256 txHash, long index, Amount amount, int height, bool coinbase, Script script, string address) 
            : this(txHash, index, amount, height, coinbase, script)
        {
            _address = address;
        }

        public static Utxo Null => new Utxo { _txHash = UInt256.Zero, _index = -1, _amount = Amount.Null, _script = Script.None };

        //public bool TryParseTxOut(ref ByteSequenceReader r, IBlockParser bp)
        //{
        //    if (!r.TryReadLittleEndian(out _value)) goto fail;

        //    bp.TxOutStart(this, r.Data.Consumed);

        //    if (!_scriptPub.TryParseScript(ref r, bp)) goto fail;

        //    bp.TxOutParsed(this, r.Data.Consumed);

        //    return true;
        //fail:
        //    return false;
        //}

        //public bool TryReadTxOut(ref ByteSequenceReader reader)
        //{
        //    if (!reader.TryReadLittleEndian(out _value)) goto fail;
        //    if (!_scriptPub.TryReadScript(ref reader)) goto fail;

        //    return true;
        //fail:
        //    return false;
        //}

        //public void Write(BinaryWriter s)
        //{
        //    s.Write(_value);
        //    _scriptPub.Write(s);
        //}

        //public void Read(BinaryReader s)
        //{
        //    _value = s.ReadInt64();
        //    _scriptPub.Read(s);
        //}

        //public override string ToString()
        //{
        //    return $"{new Amount(_value)} {_scriptPub}";
        //}

        //public IBitcoinWriter AddTo(IBitcoinWriter writer)
        //{
        //    writer
        //        .Add(_value)
        //        .Add(_scriptPub)
        //        ;
        //    return writer;
        //}
    }
}

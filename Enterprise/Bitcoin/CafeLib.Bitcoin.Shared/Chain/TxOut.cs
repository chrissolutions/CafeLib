#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Persistence;
using CafeLib.Bitcoin.Shared.Scripting;
using CafeLib.Bitcoin.Shared.Units;

namespace CafeLib.Bitcoin.Shared.Chain
{
    public struct TxOut
    {
        Int64 _value;
        Script _scriptPub;

        public Int64 Value => _value;
        public Script ScriptPub => _scriptPub;

        public static TxOut Null => new TxOut() { _value = -1 };

        public TxOut(Int64 value, Script scriptPub) { _value = value; _scriptPub = scriptPub; }

        public bool IsNull => _value == -1;

        public bool TryParseTxOut(ref ByteSequenceReader r, IBlockParser bp)
        {
            if (!r.TryReadLittleEndian(out _value)) goto fail;

            bp.TxOutStart(this, r.Data.Consumed);

            if (!_scriptPub.TryParseScript(ref r, bp)) goto fail;

            bp.TxOutParsed(this, r.Data.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadTxOut(ref ByteSequenceReader reader)
        {
            if (!reader.TryReadLittleEndian(out _value)) goto fail;
            if (!_scriptPub.TryReadScript(ref reader)) goto fail;

            return true;
        fail:
            return false;
        }

        public void Write(BinaryWriter s)
        {
            s.Write(_value);
            _scriptPub.Write(s);
        }

        public void Read(BinaryReader s)
        {
            _value = s.ReadInt64();
            _scriptPub.Read(s);
        }

        public override string ToString()
        {
            return $"{new Amount(_value)} {_scriptPub}";
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer
                .Add(_value)
 //               .Add(_scriptPub)
                ;
            return writer;
        }
    }
}

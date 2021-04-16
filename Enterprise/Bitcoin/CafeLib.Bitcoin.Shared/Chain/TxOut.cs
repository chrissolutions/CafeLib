#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.IO;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Persistence;
using CafeLib.Bitcoin.Shared.Scripting;
using CafeLib.Bitcoin.Shared.Units;

namespace CafeLib.Bitcoin.Shared.Chain
{
    public struct TxOut
    {
        private long _value;
        private Script _script;

        public long Value => _value;
        public Script Script => _script;

        public static TxOut Null => new TxOut { _value = -1 };

        public TxOut(long value, Script script)
        {
            _value = value;
            _script = script;
        }

        public bool IsNull => _value == -1;

        public bool TryParseTxOut(ref ByteSequenceReader r, IBlockParser bp)
        {
            if (!r.TryReadLittleEndian(out _value)) goto fail;

            bp.TxOutStart(this, r.Data.Consumed);

            if (!_script.TryParseScript(ref r, bp)) goto fail;

            bp.TxOutParsed(this, r.Data.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadTxOut(ref ByteSequenceReader reader)
        {
            if (!reader.TryReadLittleEndian(out _value)) goto fail;
            if (!_script.TryReadScript(ref reader)) goto fail;

            return true;
        fail:
            return false;
        }

        public void Write(BinaryWriter s)
        {
            s.Write(_value);
            _script.Write(s);
        }

        public void Read(BinaryReader s)
        {
            _value = s.ReadInt64();
            _script.Read(s);
        }

        public override string ToString()
        {
            return $"{new Amount(_value)} {_script}";
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer
                .Add(_value)
                .Add(_script)
                ;
            return writer;
        }
    }
}

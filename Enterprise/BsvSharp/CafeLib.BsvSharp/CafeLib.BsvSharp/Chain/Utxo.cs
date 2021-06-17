#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;

namespace CafeLib.BsvSharp.Chain
{
    public class Utxo
    {
        UInt32 _heightAndIsCoinbase;

        public TxOut Out;

        public bool IsCoinbase 
        {
            get => (_heightAndIsCoinbase & 1) == 1;
            set => _heightAndIsCoinbase = (uint)(Height << 1) + (value ? 1u : 0u);
        }

        public int Height
        {
            get => (int)(_heightAndIsCoinbase >> 1);
            set => _heightAndIsCoinbase = (uint)(value << 1) + (IsCoinbase ? 1u : 0u);
        }

        public void Write(BinaryWriter s)
        {
            s.Write(_heightAndIsCoinbase);
            Out.Write(s);
        }

        public void Read(BinaryReader s)
        {
            _heightAndIsCoinbase = s.ReadUInt32();
            Out.Read(s);
        }
    }
}
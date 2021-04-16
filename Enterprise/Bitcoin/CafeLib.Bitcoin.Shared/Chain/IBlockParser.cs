#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Shared.Scripting;

namespace CafeLib.Bitcoin.Shared.Chain 
{
    public interface IBlockParser 
    {
        //void BlockStart(KzBlockHeader bh, long offset);
        //void BlockParsed(KzBlockHeader bh, long offset);
        //void TxStart(KzTransaction t, long offset);
        //void TxParsed(KzTransaction t, long offset);
        void TxOutStart(TxOut to, long offset);
        void TxOutParsed(TxOut to, long offset);
        //void TxInStart(KzTxIn ti, long offset);
        //void TxInParsed(KzTxIn ti, long offset);
        void ScriptStart(Script s, long offset);
        void ScriptParsed(Script s, long offset);
    }
}

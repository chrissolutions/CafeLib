﻿using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Extensions
{
    public static class WriterExtensions
    {
        public static IBitcoinWriter Add(this IBitcoinWriter w, Script script, bool withoutCodeSeparators = false)
            => script.AddTo(w, withoutCodeSeparators);

        public static IBitcoinWriter Add(this IBitcoinWriter w, OutPoint op) => op.AddTo(w);
        public static IBitcoinWriter Add(this IBitcoinWriter w, TxIn txIn) => txIn.AddTo(w);
        public static IBitcoinWriter Add(this IBitcoinWriter w, TxOut txOut) => txOut.AddTo(w);
        public static IBitcoinWriter Add(this IBitcoinWriter w, Transaction tx) => tx.AddTo(w);
        public static IBitcoinWriter Add(this IBitcoinWriter w, Operand op) => op.AddTo(w);
    }
}
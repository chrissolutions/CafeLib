using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.Bitcoin.Shared.Chain;
using CafeLib.Bitcoin.Shared.Persistence;
using CafeLib.Bitcoin.Shared.Scripting;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class WriterExtensions
    {
        public static IBitcoinWriter Add(this IBitcoinWriter w, Script script, bool withoutCodeSeparators = false) => script.AddTo(w, withoutCodeSeparators);
        public static IBitcoinWriter Add(this IBitcoinWriter w, OutPoint op) => op.AddTo(w);
        //public static IBitcoinWriter Add(this IBitcoinWriter w, KzTxIn txIn) => txIn.AddTo(w);
        //public static IBitcoinWriter Add(this IBitcoinWriter w, KzTxOut txOut) => txOut.AddTo(w);
        public static IBitcoinWriter Add(this IBitcoinWriter w, Transaction tx) => tx.AddTo(w);
        public static IBitcoinWriter Add(this IBitcoinWriter w, Operand op) => op.AddTo(w);

        //public static KzIWriter Add<T>(this KzIWriter w, T[] vs) { foreach (var v in vs) v.Add(w); return w; }
    }
}

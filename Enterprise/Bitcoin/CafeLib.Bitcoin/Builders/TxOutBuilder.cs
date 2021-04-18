#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Units;

namespace CafeLib.Bitcoin.Builders
{
    public class TxOutBuilder
    {
        public Amount Value { get; private set; }
        public ScriptBuilder ScriptPubBuilder { get; private set; }
        public PublicKey PubKey { get; private set; }

        public TxOutBuilder()
        {
            ScriptPubBuilder = new ScriptBuilder();
        }

        public TxOutBuilder(TxOut txOut)
        {
            Value = txOut.Value;
            ScriptPubBuilder.Set(txOut.Script);
        }

        public static TxOutBuilder ToPay2PublicKeyHash(PublicKey pubKey, Amount value)
        {
            var pub = new ScriptPubPay2PublicKeyHashBuilder(pubKey.ToHash160());

            var r = new TxOutBuilder {
                Value = value,
                ScriptPubBuilder = pub,
                PubKey = pubKey
            };
            return r;
        }

        public TxOut ToTxOut()
        {
            return new TxOut(Value, ScriptPubBuilder);
        }

        public static implicit operator TxOut(TxOutBuilder rhs) => rhs.ToTxOut();
        public static explicit operator TxOutBuilder(TxOut rhs) => new TxOutBuilder(rhs);
    }
}

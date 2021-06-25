#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics;
using CafeLib.Bitcoin.Units;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Builders
{
    public class TxInBuilder
    {
        private PublicKey _pubKey;

        /// <summary>
        /// Must be set if PrivateKey is not provided until signing operation.
        /// </summary>
        public PublicKey PublicKey
        {
            get => PrivateKey?.CreatePublicKey() ?? _pubKey; 
            set => _pubKey = value;
        }

        /// <summary>
        /// If provided, PubKey does not need to be set. Signing can be done without providing it again.
        /// </summary>
        public PrivateKey PrivateKey { get; set; }

        public UInt256 PrevOutHashTx;
        public int PrevOutIndex;
        public ScriptBuilder ScriptSig = new ScriptBuilder();
        public uint Sequence = TxIn.SequenceFinal;
        public UInt256 TxHash;

        /// <summary>
        /// This is the Value of the referenced Prevout.
        /// The amount this input contributes to funding this transaction.
        /// </summary>
        public Amount? Value;

        /// <summary>
        /// This is the ScriptPub of the referenced Prevout.
        /// Used to sign and verify this input.
        /// </summary>
        public ScriptBuilder ScriptPub;

        /// <summary>
        /// The funding transaction referenced by HashTx.
        /// It may not be available and the value is null in some circumstances.
        /// </summary>
        public Transaction PrevOutTx;

        public OutPoint PrevOut
        {
            get => new OutPoint(PrevOutHashTx, PrevOutIndex);
            set
            {
                PrevOutHashTx = value.TxId; 
                PrevOutIndex = value.Index;
            }
        }

        public TxInBuilder() { }

        public TxInBuilder(TxIn txIn)
        {
            PrevOutHashTx = txIn.PrevOut.TxId;
            PrevOutIndex = txIn.PrevOut.Index;
            ScriptSig.Set(txIn.ScriptSig);
            Sequence = txIn.Sequence;
        }

        public static TxInBuilder FromPay2PublicKeyHash(PrivateKey privKey, Amount value, UInt256 hashTx, int index, Script scriptPub, UInt32 sequence = TxIn.SequenceFinal)
        {
            var r = FromPay2PublicKeyHash(privKey.CreatePublicKey(), value, hashTx, index, scriptPub, sequence);
            r.PrivateKey = privKey;
            return r;
        }

        public static TxInBuilder FromPay2PublicKeyHash(PublicKey pubKey, Amount value, UInt256 prevOutHashTx, int prevOutIndex, Script scriptPub, UInt32 sequence = TxIn.SequenceFinal)
        {
            var pubKeyHash = new ScriptSigPay2PublicKeyHashBuilder(pubKey);
            var signatureHash = new ScriptSigPay2PublicKeyHashBuilder(pubKey);
            var script = new Script(pubKeyHash.ToBytes());
            Debug.Assert(script == scriptPub);

            var r = new TxInBuilder {
                PrevOutHashTx = prevOutHashTx,
                PrevOutIndex = prevOutIndex,
                ScriptSig = signatureHash,
                Sequence = sequence,
                Value = value,
                ScriptPub = pubKeyHash,
                PublicKey = pubKey
            };
            return r;
        }

        public static TxInBuilder FromPay2PublicKeyHash(PublicKey pubKey, Transaction prevOutTx, int prevOutN, UInt32 sequence = TxIn.SequenceFinal)
        {
            Debug.Assert(prevOutTx.Hash != UInt256.Zero);
            var o = prevOutTx.Outputs[prevOutN];
            var r = FromPay2PublicKeyHash(pubKey, o.Value, prevOutTx.Hash, prevOutN, o.Script, sequence);
            r.PrevOutTx = prevOutTx;
            return r;
        }

        public TxIn ToTxIn()
        {
            return new TxIn(PrevOut, ScriptSig, Sequence);
        }

        public static implicit operator TxIn(TxInBuilder rhs) => rhs.ToTxIn();
        public static explicit operator TxInBuilder(TxIn rhs) => new TxInBuilder(rhs);
    }
}

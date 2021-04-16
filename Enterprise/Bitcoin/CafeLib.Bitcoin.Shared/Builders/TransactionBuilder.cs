#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Bitcoin.Shared.Chain;
using CafeLib.Bitcoin.Shared.Keys;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Scripting;
using CafeLib.Bitcoin.Shared.Units;

namespace CafeLib.Bitcoin.Shared.Builders
{

    /// <summary>
    /// Support dynamic construction of new Bitcoin transactions.
    /// See <see cref="Transaction"/> for serializing and sending.
    /// </summary>
    public class TransactionBuilder
    {
        public int Version = 1;
        public List<TxInBuilder> Vin = new List<TxInBuilder>();
        public List<TxOutBuilder> Vout = new List<TxOutBuilder>();
        public UInt32 LockTime = 0;

        public UInt256? HashTx;

        public Amount? CurrentFee => Vin.Where(i => i.PrevOut.Index >= 0).Sum(i => i.Value) - Vout.Sum(o => o.Value);

        public TransactionBuilder() { }

        public TransactionBuilder(Transaction tx)
        {
            Version = tx.Version;
            Vin = tx.Inputs.Select(x => (TxInBuilder)x).ToList();
            Vout = tx.Outputs.Select(x => (TxOutBuilder)x).ToList();
            LockTime = tx.LockTime;
            HashTx = tx.Hash;
            if (HashTx.Value == UInt256.Zero) HashTx = null;
        }

        public static TransactionBuilder Pay2PublicKeyHash
            ( IEnumerable<(PublicKey pubKey, long value, byte[] hashTxBytes, int index, byte[] scriptPub)> from
            , IEnumerable<(PublicKey pubKey, long value)> to
            )
        {
            var r = new TransactionBuilder();
            foreach (var (pubKey, value, hashTxBytes, index, scriptPub) in from) r.AddInPay2PublicKeyHash(pubKey, value, new UInt256(hashTxBytes), index, new Script(scriptPub));
            foreach (var (pubKey, value) in to) r.AddOutPay2PublicKeyHash(pubKey, value);
            return r;
        }

        public void AddInPay2PublicKeyHash(PublicKey pubKey, Amount value, UInt256 txId, int index, Script scriptPub, UInt32 sequence = TxIn.SequenceFinal)
        {
            Vin.Add(TxInBuilder.FromPay2PublicKeyHash(pubKey, value, txId, index, scriptPub, sequence));
        }

        public static TransactionBuilder Pay2PublicKeyHash(IEnumerable<(PublicKey pubKey, Transaction tx, int index)> from, IEnumerable<(PublicKey pubKey, long value)> to)
        {
            var r = new TransactionBuilder();
            foreach (var (pubKey, tx, index) in from) r.AddInPay2PublicKeyHash(pubKey, tx, index);
            foreach (var (pubKey, value) in to) r.AddOutPay2PublicKeyHash(pubKey, value);
            return r;
        }

        public void AddInPay2PublicKeyHash(PublicKey pubKey, Transaction tx, int n, UInt32 sequence = TxIn.SequenceFinal)
        {
            Vin.Add(TxInBuilder.FromPay2PublicKeyHash(pubKey, tx, n, sequence));
        }

        public void AddIn((Transaction tx, TxOut o, int i) txOut)
        {
            throw new NotImplementedException();
        }

        public void AddIn(OutPoint prevout, Script scriptSig, UInt32 sequence = TxIn.SequenceFinal)
        {
            throw new NotImplementedException();
            //Vin.Add(new KzTxInBuilder { Prevout = prevout, ScriptSig., sequence));
        }

        public void AddOutPay2PublicKeyHash(PublicKey pubKey, Amount value)
        {
            Vout.Add(TxOutBuilder.ToPay2PublicKeyHash(pubKey, value));
        }

        public void AddOut(Script scriptPubKey, long nValue)
        {
            throw new NotImplementedException();
            //Vout.Add(new KzTxOut(nValue, scriptPubKey));
        }

        //public Transaction ToTransaction() => new Transaction(this);

        public ReadOnlySequence<byte> ToReadOnlySequence()
        {
            throw new NotImplementedException();
            //return new ReadOnlySequence<byte>();
        }

        public ReadOnlySequence<byte> ToSequence()
        {
            throw new NotImplementedException();
            //return new ReadOnlySequence<byte>();
        }

        /// <summary>
        /// Find all the OP_RETURN outputs followed by a matching protocol identifier pushdata (20 bytes long).
        /// For each match, return the TxOutBuilder and a trimmed array the remaining ScriptPub KzBOp's.
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        //public IEnumerable<(TxOutBuilder o, OperandBuilder[] data)> FindPushDataByProtocol(UInt160 protocol)
        //{
        //    var val = protocol.ToBytes();

        //    foreach (var o in Vout) {
        //        var ops = o.ScriptPubBuilder.Ops;
        //        if (ops.Count > 2
        //            && ops[0].Operand.Code == Opcode.OP_RETURN
        //            && ops[1].Operand.Code == Opcode.OP_PUSH20
        //            && ops[1].Operand.Data.Sequence.CompareTo(val) == 0)
        //            yield return (o, ops.Skip(2).ToArray());
        //    }
        //}

        //public bool CheckSignatures(IEnumerable<PrivateKey> privKeys = null) {
        //    return Sign(privKeys, confirmExistingSignatures: true);
        //}

//        public bool Sign(IEnumerable<PrivateKey> privKeys = null, bool confirmExistingSignatures = false)
//        {
//            var signedOk = true;
//            var sigHashType = new KzSigHashType(KzSigHash.ALL | KzSigHash.FORKID);
//            var tx = ToTransaction();
//            var nIn = -1;
//            foreach (var i in Vin) {
//                nIn++;
//                var scriptSig = i.ScriptSig;
//                if (scriptSig.Ops.Count == 2) {
//                    var pubKey = new KzPubKey();
//                    pubKey.Set(scriptSig.Ops[1].Op.Data.ToSpan());
//                    if (pubKey.IsValid) {
//                        var privKey = i.PrivKey ?? privKeys?.FirstOrDefault(k => k.GetPubKey() == pubKey);
//                        if (privKey != null) {
//                            var value = i.Value ?? i.PrevOutTx?.Vout[i.PrevOutN].Value ?? 0L;
//                            var sigHash = KzScriptInterpreter.ComputeSignatureHash(i.ScriptPub, tx, nIn, sigHashType, value, KzScriptFlags.ENABLE_SIGHASH_FORKID);
//                            var (ok, sig) = privKey.Sign(sigHash);
//                            if (ok) {
//                                var sigWithType = new byte[sig.Length + 1];
//                                sig.CopyTo(sigWithType.AsSpan());
//                                sigWithType[^1] = (byte)sigHashType.rawSigHashType;
//                                var op = KzOp.Push(sigWithType.AsSpan());
//                                if (confirmExistingSignatures)
//                                    signedOk &= op == scriptSig.Ops[0].Op;
//                                else
//                                    scriptSig.Ops[0] = op;
//                            } else signedOk = false;
//                        } else signedOk = false;
//                    } else signedOk = false;
//                }
//            }
//            return signedOk;
//#if false
//            var tx = txb.ToTransaction();
//                var bytes = tx.ToBytes();
//                var hex = bytes.ToHex();

//                var (ok, sig) = privKey1h11.Sign(sigHash);
//            //var (ok, sig) = privKey1h11.SignCompact(sigHash);
//            if (ok) {
//                var sigWithType = new byte[sig.Length + 1];
//                sig.CopyTo(sigWithType.AsSpan());
//                sigWithType[^1] = (byte)sigHashType.rawSigHashType;
//                txb.Vin[0].ScriptSig.Ops[0] = KzOp.Push(sigWithType.AsSpan());
//            }
//#endif
//        }
    }


}

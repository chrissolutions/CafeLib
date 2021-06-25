#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CafeLib.Bitcoin.Units;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Scripting
{
    public static class ScriptInterpreter
    {
        private static readonly SignatureCheckerBase DefaultSignatureChecker = new SignatureCheckerBase();

        public static ScriptFlags ParseFlags(string flags)
        {
#if false
            var map = new Dictionary<string, KzScriptFlags>();
            map.Add("NONE", KzScriptFlags.VERIFY_NONE);
            map.Add("P2SH", KzScriptFlags.VERIFY_P2SH);
            map.Add("STRICTENC", KzScriptFlags.VERIFY_STRICTENC);
            map.Add("DERSIG", KzScriptFlags.VERIFY_DERSIG);
            map.Add("LOW_S", KzScriptFlags.VERIFY_LOW_S);
            map.Add("SIGPUSHONLY", KzScriptFlags.VERIFY_SIGPUSHONLY);
            map.Add("MINIMALDATA", KzScriptFlags.VERIFY_MINIMALDATA);
            map.Add("NULLDUMMY", KzScriptFlags.VERIFY_NULLDUMMY);
            map.Add("DISCOURAGE_UPGRADABLE_NOPS", KzScriptFlags.VERIFY_DISCOURAGE_UPGRADABLE_NOPS);
            map.Add("CLEANSTACK", KzScriptFlags.VERIFY_CLEANSTACK);
            map.Add("MINIMALIF", KzScriptFlags.VERIFY_MINIMALIF);
            map.Add("NULLFAIL", KzScriptFlags.VERIFY_NULLFAIL);
            map.Add("CHECKLOCKTIMEVERIFY", KzScriptFlags.VERIFY_CHECKLOCKTIMEVERIFY);
            map.Add("CHECKSEQUENCEVERIFY", KzScriptFlags.VERIFY_CHECKSEQUENCEVERIFY);
            map.Add("COMPRESSED_PUBKEYTYPE", KzScriptFlags.VERIFY_COMPRESSED_PUBKEYTYPE);
            map.Add("SIGHASH_FORKID", KzScriptFlags.ENABLE_SIGHASH_FORKID);
            map.Add("REPLAY_PROTECTION", KzScriptFlags.ENABLE_REPLAY_PROTECTION);
#endif
            var fs = flags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return fs.Select(f => Enum.GetNames(typeof(ScriptFlags))
                .Single(n => n.Contains(f)))
                .Select(Enum.Parse<ScriptFlags>)
                .Aggregate((ScriptFlags)0, (current, sf) => current | sf);
        }

        public static UInt256 ComputeSignatureHash
        (
            Script scriptCode,
            Transaction txTo,
            int nIn,
            SignatureHashType sigHashType,
            Amount amount,
            ScriptFlags flags = ScriptFlags.ENABLE_SIGHASH_FORKID
        )
        {
            if (sigHashType.HasForkId && (flags & ScriptFlags.ENABLE_SIGHASH_FORKID) != 0)
            {
                var hashPrevOuts = new UInt256();
                var hashSequence = new UInt256();
                var hashOutputs = new UInt256();

                if (!sigHashType.HasAnyoneCanPay)
                {
                    hashPrevOuts = GetPrevOutHash(txTo);
                }

                var baseNotSingleOrNone =
                    (sigHashType.GetBaseType() != BaseSignatureHashEnum.Single) &&
                    (sigHashType.GetBaseType() != BaseSignatureHashEnum.None);

                if (!sigHashType.HasAnyoneCanPay && baseNotSingleOrNone)
                {
                    hashSequence = GetSequenceHash(txTo);
                }

                if (baseNotSingleOrNone)
                {
                    hashOutputs = GetOutputsHash(txTo);
                }
                else if (sigHashType.GetBaseType() == BaseSignatureHashEnum.Single && nIn < txTo.Outputs.Length)
                {
                    using var hw = new HashWriter();
                    hw.Add(txTo.Outputs[nIn]);
                    hashOutputs = hw.GetHashFinal();
                }

                using var writer = new HashWriter();
                writer
                    // Version
                    .Add(txTo.Version)
                    // Input prevouts/nSequence (none/all, depending on flags)
                    .Add(hashPrevOuts)
                    .Add(hashSequence)
                    // The input being signed (replacing the scriptSig with scriptCode +
                    // amount). The prevout may already be contained in hashPrevout, and the
                    // nSequence may already be contain in hashSequence.
                    .Add(txTo.Inputs[nIn].PrevOut)
                    .Add(scriptCode)
                    .Add(amount.Satoshis)
                    .Add(txTo.Inputs[nIn].Sequence)
                    // Outputs (none/one/all, depending on flags)
                    .Add(hashOutputs)
                    // Locktime
                    .Add(txTo.LockTime)
                    // Sighash type
                    .Add(sigHashType.RawSigHashType)
                    ;
                return writer.GetHashFinal();
            }

            if (nIn >= txTo.Inputs.Length)
            {
                //  nIn out of range
                return UInt256.One;
            }

            // Check for invalid use of SIGHASH_SINGLE
            if (sigHashType.GetBaseType() == BaseSignatureHashEnum.Single && nIn >= txTo.Outputs.Length)
            {
                //  nOut out of range
                return UInt256.One;
            }

            {
                // Original digest algorithm...
                var hasAnyoneCanPay = sigHashType.HasAnyoneCanPay;
                // ReSharper disable once UnusedVariable
                var numberOfInputs = hasAnyoneCanPay ? 1 : txTo.Inputs.Length;
                using var writer = new HashWriter();
                // Start with the version...
                writer.Add(txTo.Version);
                // Add Input(s)...
                if (hasAnyoneCanPay)
                {
                    // AnyoneCanPay serializes only the input being signed.
                    var i = txTo.Inputs[nIn];
                    writer
                        .Add((byte)1)
                        .Add(i.PrevOut)
                        .Add(scriptCode, true)
                        .Add(i.Sequence);
                }
                else
                {
                    // Non-AnyoneCanPay case. Process all inputs but handle input being signed in its own way.
                    var isSingleOrNone = sigHashType.IsBaseSingle || sigHashType.IsBaseNone;
                    writer.Add(txTo.Inputs.Length.AsVarIntBytes());
                    for (var nInput = 0; nInput < txTo.Inputs.Length; nInput++)
                    {
                        var i = txTo.Inputs[nInput];
                        writer.Add(i.PrevOut);
                        if (nInput != nIn)
                            writer.Add(Script.None);
                        else
                            writer.Add(scriptCode, true);
                        if (nInput != nIn && isSingleOrNone)
                            writer.Add(0);
                        else
                            writer.Add(i.Sequence);
                    }
                }
                // Add Output(s)...
                var nOutputs = sigHashType.IsBaseNone ? 0 : sigHashType.IsBaseSingle ? nIn + 1 : txTo.Outputs.Length;
                writer.Add(nOutputs.AsVarIntBytes());
                for (var nOutput = 0; nOutput < nOutputs; nOutput++)
                {
                    if (sigHashType.IsBaseSingle && nOutput != nIn)
                        writer.Add(TxOut.Null);
                    else
                        writer.Add(txTo.Outputs[nOutput]);
                }
                // Finish up...
                writer
                    .Add(txTo.LockTime)
                    .Add(sigHashType.RawSigHashType)
                    ;
                return writer.GetHashFinal();
            }
        }

        public static UInt256 GetPrevOutHash(Transaction txTo)
        {
            using var hw = new HashWriter();
            foreach (var i in txTo.Inputs)
            {
                hw.Add(i.PrevOut);
            }

            return hw.GetHashFinal();
        }

        public static UInt256 GetSequenceHash(Transaction txTo)
        {
            using var hw = new HashWriter();
            foreach (var i in txTo.Inputs)
            {
                hw.Add(i.Sequence);
            }

            return hw.GetHashFinal();
        }

        public static UInt256 GetOutputsHash(Transaction txTo)
        {
            using var hw = new HashWriter();
            foreach (var o in txTo.Outputs)
            {
                hw.Add(o);
            }

            return hw.GetHashFinal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SetSuccess(out ScriptError ret)
        {
            ret = ScriptError.OK;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SetError(out ScriptError output, ScriptError input)
        {
            output = input;
            return false;
        }

        /// <summary>
        /// Modeled on Bitcoin-SV interpreter.cpp 0.1.1 lines 1866-1945
        /// </summary>
        /// <param name="scriptSig"></param>
        /// <param name="scriptPub"></param>
        /// <param name="flags"></param>
        /// <param name="checker"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool VerifyScript(Script scriptSig, Script scriptPub, ScriptFlags flags, SignatureCheckerBase checker, out ScriptError error)
        {
            SetError(out error, ScriptError.UNKNOWN_ERROR);

            if ((flags & ScriptFlags.ENABLE_SIGHASH_FORKID) != 0)
            {
                flags |= ScriptFlags.VERIFY_STRICTENC;
            }

            if ((flags & ScriptFlags.VERIFY_SIGPUSHONLY) != 0 && !scriptSig.IsPushOnly())
            {
                return SetError(out error, ScriptError.SIG_PUSHONLY);
            }

            var evaluator = new ScriptEvaluator();
            return evaluator switch
            {
                _ when !evaluator.EvalScript(scriptSig, flags, checker, out error) => false,
                _ when !evaluator.EvalScript(scriptPub, flags, checker, out error) => false,
                _ when evaluator.Count == 0 => SetError(out error, ScriptError.EVAL_FALSE),
                _ when !evaluator.Peek() => SetError(out error, ScriptError.EVAL_FALSE),
                _ => SetSuccess(out error)
            };
        }
    }
}

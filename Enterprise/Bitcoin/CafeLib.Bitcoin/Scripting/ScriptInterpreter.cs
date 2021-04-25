#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;
using CafeLib.Bitcoin.Services;
using CafeLib.Bitcoin.Units;

namespace CafeLib.Bitcoin.Scripting
{
    public static class ScriptInterpreter
    {
        private static SignatureHashType GetHashType(VarType vchSig) 
            => new SignatureHashType(vchSig.Length == 0 ? SignatureHashEnum.Unsupported : (SignatureHashEnum)vchSig.LastByte);

        private static readonly SignatureCheckerBase DefaultSignatureChecker = new SignatureCheckerBase();

        private static void CleanupScriptCode(Script scriptCode, VarType vchSig, ScriptFlags flags)
        {
            // Drop the signature in scripts when SIGHASH_FORKID is not used.
            var sigHashType = GetHashType(vchSig);
            if ((flags & ScriptFlags.ENABLE_SIGHASH_FORKID) == 0 || !sigHashType.HasForkId) {
                scriptCode.FindAndDelete(vchSig);
            }
        }

        private static bool CheckPubKeyEncoding(VarType vchPubKey, ScriptFlags flags, ref ScriptError error)
        {
            if ((flags & ScriptFlags.VERIFY_STRICTENC) != 0 && !IsCompressedOrUncompressedPubKey(vchPubKey))
                return SetError(out error, ScriptError.PUBKEYTYPE);

            // Only compressed keys are accepted when
            // SCRIPT_VERIFY_COMPRESSED_PUBKEYTYPE is enabled.

            if ((flags & ScriptFlags.VERIFY_COMPRESSED_PUBKEYTYPE) != 0 && !IsCompressedPubKey(vchPubKey))
                return SetError(out error, ScriptError.NONCOMPRESSED_PUBKEY);

            return true;
        }

        private static bool IsCompressedOrUncompressedPubKey(VarType vchPubKey)
        {
            var length = vchPubKey.Length;
            var first = vchPubKey.FirstByte;

            if (length < 33) {
                //  Non-canonical public key: too short
                return false;
            }
            if (first == 0x04) {
                if (length != 65) {
                    //  Non-canonical public key: invalid length for uncompressed key
                    return false;
                }
            } else if (first == 0x02 || first == 0x03) {
                if (length != 33) {
                    //  Non-canonical public key: invalid length for compressed key
                    return false;
                }
            } else {
                //  Non-canonical public key: neither compressed nor uncompressed
                return false;
            }
            return true;
        }

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
                .Aggregate((ScriptFlags) 0, (current, sf) => current | sf);
        }

        private static bool IsCompressedPubKey(VarType vchPubKey)
        {
            var length = vchPubKey.Length;
            var first = vchPubKey.FirstByte;

            return length == 33 && (first == 0x02 || first == 0x03);
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

                if (!sigHashType.HasAnyoneCanPay) {
                    hashPrevOuts = GetPrevOutHash(txTo);
                }

                var baseNotSingleOrNone =
                    (sigHashType.GetBaseType() != BaseSignatureHashEnum.Single) &&
                    (sigHashType.GetBaseType() != BaseSignatureHashEnum.None);

                if (!sigHashType.HasAnyoneCanPay && baseNotSingleOrNone) {
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
                    .Add(sigHashType.rawSigHashType)
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
                    for (var nInput = 0; nInput < txTo.Inputs.Length; nInput++) {
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
                for (var nOutput = 0; nOutput < nOutputs; nOutput++) {
                    if (sigHashType.IsBaseSingle && nOutput != nIn)
                        writer.Add(TxOut.Null);
                    else
                        writer.Add(txTo.Outputs[nOutput]);
                }
                // Finish up...
                writer
                    .Add(txTo.LockTime)
                    .Add(sigHashType.rawSigHashType)
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

        private static bool CheckSignatureEncoding(VarType vchSig, ScriptFlags flags, ref ScriptError error)
        {
            // Empty signature. Not strictly DER encoded, but allowed to provide a
            // compact way to provide an invalid signature for use with CHECK(MULTI)SIG
            if (vchSig.Length == 0) return true;

            if ((flags & (ScriptFlags.VERIFY_DERSIG | ScriptFlags.VERIFY_LOW_S | ScriptFlags.VERIFY_STRICTENC)) != 0
                && !IsValidSignatureEncoding(vchSig)) 
            {
                return SetError(out error, ScriptError.SIG_DER);
            }

            if ((flags & ScriptFlags.VERIFY_LOW_S) != 0 && !IsLowDerSignature(vchSig, ref error)) 
            {
                // error is set
                return false;
            }

            if ((flags & ScriptFlags.VERIFY_STRICTENC) != 0) 
            {
                var ht = GetHashType(vchSig);
                if (!ht.IsDefined) return SetError(out error, ScriptError.SIG_HASHTYPE);
                var usesForkId = ht.HasForkId;
                var forkIdEnabled = (flags & ScriptFlags.ENABLE_SIGHASH_FORKID) != 0;
                if (!forkIdEnabled && usesForkId) return SetError(out error, ScriptError.ILLEGAL_FORKID);
                if (forkIdEnabled && !usesForkId) return SetError(out error, ScriptError.MUST_USE_FORKID);
            }

            return true;
        }

        private static bool IsLowDerSignature(VarType vchSig, ref ScriptError error)
        {
            if (!IsValidSignatureEncoding(vchSig)) return SetError(out error, ScriptError.SIG_DER);

            var sigInput = vchSig.Slice(0, (int)vchSig.Length - 1);

            if (!PublicKey.CheckLowS(sigInput)) return SetError(out error, ScriptError.SIG_HIGH_S);

            return true;
        }

        /**
         * A canonical signature exists of: <30> <total len> <02> <len R> <R> <02> <len
         * S> <S> <hashtype>, where R and S are not negative (their first byte has its
         * highest bit not set), and not excessively padded (do not start with a 0 byte,
         * unless an otherwise negative number follows, in which case a single 0 byte is
         * necessary and even required).
         *
         * See https://bitcointalk.org/index.php?topic=8392.msg127623#msg127623
         *
         * This function is consensus-critical since BIP66.
         */
        private static bool IsValidSignatureEncoding(VarType vchSig)
        {
            // Format: 0x30 [total-length] 0x02 [R-length] [R] 0x02 [S-length] [S]
            // [sighash]
            // * total-length: 1-byte length descriptor of everything that follows,
            // excluding the sighash byte.
            // * R-length: 1-byte length descriptor of the R value that follows.
            // * R: arbitrary-length big-endian encoded R value. It must use the
            // shortest possible encoding for a positive integers (which means no null
            // bytes at the start, except a single one when the next byte has its
            // highest bit set).
            // * S-length: 1-byte length descriptor of the S value that follows.
            // * S: arbitrary-length big-endian encoded S value. The same rules apply.
            // * sighash: 1-byte value indicating what data is hashed (not part of the
            // DER signature)

            // Minimum and maximum size constraints.
            var length = vchSig.Length;
            if (length < 9) return false;
            if (length > 73) return false;

            var sig = vchSig.ToSpan();

            // A signature is of type 0x30 (compound).
            if (sig[0] != 0x30) return false;

            // Make sure the length covers the entire signature.
            if (sig[1] != sig.Length - 3) return false;

            // Extract the length of the R element.
            var lenR = sig[3];

            // Make sure the length of the S element is still inside the signature.
            if (5 + lenR >= sig.Length) return false;

            // Extract the length of the S element.
            var lenS = sig[5 + lenR];

            // Verify that the length of the signature matches the sum of the length
            // of the elements.
            if (lenR + lenS + 7 != sig.Length) return false;

            // Check whether the R element is an integer.
            if (sig[2] != 0x02) return false;

            // Zero-length integers are not allowed for R.
            if (lenR == 0) return false;

            // Negative numbers are not allowed for R.
            if ((sig[4] & 0x80) != 0) return false;

            // Null bytes at the start of R are not allowed, unless R would otherwise be
            // interpreted as a negative number.
            if (lenR > 1 && (sig[4] == 0) && (sig[5] & 0x80) == 0) return false;

            // Check whether the S element is an integer.
            if (sig[lenR + 4] != 0x02) return false;

            // Zero-length integers are not allowed for S.
            if (lenS == 0) return false;

            // Negative numbers are not allowed for S.
            if ((sig[lenR + 6] & 0x80) != 0) return false;

            // Null bytes at the start of S are not allowed, unless S would otherwise be
            // interpreted as a negative number.
            if (lenS > 1 && (sig[lenR + 6] == 0x00) && (sig[lenR + 7] & 0x80) == 0) {
                return false;
            }

            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool SetSuccess(out ScriptError ret)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidMaxOpsPerScript(int nOpCount) => nOpCount <= RootService.Network.Consensus.MaxOperationsPerScript;

        private static bool IsOpcodeDisabled(Opcode opcode, ScriptFlags flags)
        {
            return opcode switch
            {
                // Disabled opcodes.
                Opcode.OP_2MUL => true,
                // Disabled opcodes.
                Opcode.OP_2DIV =>true,
                _ => false
            };
        }

        private static bool CheckMinimalPush(ref Operand op)
        {
            var opcode = op.Code;
            var dataSize = op.Data.Length;
            if (dataSize == 0) {
                // Could have used OP_0.
                return opcode == Opcode.OP_0;
            }

            var b0 = op.Data.Sequence.Data.First.Span[0];
            if (dataSize == 1 && b0 >= 1 && b0 <= 16) 
            {
                // Could have used OP_1 .. OP_16.
                return (int)opcode == (int)Opcode.OP_1 + (b0 - 1);
            }

            if (dataSize == 1 && b0 == 0x81) 
            {
                // Could have used OP_1NEGATE.
                return opcode == Opcode.OP_1NEGATE;
            }

            if (dataSize <= 75) {
                // Could have used a direct push (opcode indicating number of bytes
                // pushed + those bytes).
                return (int)opcode == dataSize;
            }

            if (dataSize <= 255) 
            {
                // Could have used OP_PUSHDATA.
                return opcode == Opcode.OP_PUSHDATA1;
            }

            if (dataSize <= 65535) 
            {
                // Could have used OP_PUSHDATA2.
                return opcode == Opcode.OP_PUSHDATA2;
            }

            return true;
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

            if ((flags & ScriptFlags.ENABLE_SIGHASH_FORKID) != 0) {
                flags |= ScriptFlags.VERIFY_STRICTENC;
            }

            if ((flags & ScriptFlags.VERIFY_SIGPUSHONLY) != 0 && !scriptSig.IsPushOnly())
            {
                return SetError(out error, ScriptError.SIG_PUSHONLY);
            }

            var stack = new ScriptStack<VarType>();
            if (!EvalScript(stack, scriptSig, flags, checker, out error)) return false;
            if (!EvalScript(stack, scriptPub, flags, checker, out error)) return false;
            if (stack.Count == 0) return SetError(out error, ScriptError.EVAL_FALSE);
            if (stack.Peek().ToBool() == false) return SetError(out error, ScriptError.EVAL_FALSE);

            return SetSuccess(out error);
        }

        private static readonly VarType VchZero = new VarType(new byte[0]);
        private static readonly VarType VchFalse = new VarType(new byte[0]);
        private static readonly VarType VchTrue = new VarType(new byte[] { 1 });

        /// <summary>
        /// Modeled on Bitcoin-SV interpreter.cpp 0.1.1 lines 384-1520
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="script"></param>
        /// <param name="flags"></param>
        /// <param name="checker"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool EvalScript(ScriptStack<VarType> stack, Script script, ScriptFlags flags, SignatureCheckerBase checker, out ScriptError error)
        {
            var ros = script.Data.Sequence;
            var pc = ros.Data.Start;
            var pend = ros.Data.End;
            var beginCodeHash = ros.Data.Start;
            var op = new Operand();
            var vfExec = new ScriptStack<bool>();
            var altStack = new ScriptStack<VarType>();
            checker ??= DefaultSignatureChecker;

            SetError(out error, ScriptError.UNKNOWN_ERROR);

            if (script.Length > RootService.Network.Consensus.MaxScriptSize)
                return SetError(out error, ScriptError.SCRIPT_SIZE);

            var nOpCount = 0;
            var fRequireMinimal = (flags & ScriptFlags.VERIFY_MINIMALDATA) != 0;

            try 
            {
                while (ros.Length > 0) 
                {
                    var fExec = vfExec.Contains(false) == false;

                    if (!op.TryReadOperand(ref ros)) 
                    {
                        return SetError(out error, ScriptError.BAD_OPCODE);
                    }

                    if (op.Data.Length > RootService.Network.Consensus.MaxScriptElementSize) 
                    {
                        return SetError(out error, ScriptError.PUSH_SIZE);
                    }

                    if (op.Code > Opcode.OP_16) 
                    {
                        ++nOpCount;
                        if (!IsValidMaxOpsPerScript(nOpCount))
                        {
                            return SetError(out error, ScriptError.OP_COUNT);
                        }
                    }

                    // Some opcodes are disabled.
                    if (IsOpcodeDisabled(op.Code, flags)) 
                    {
                        return SetError(out error, ScriptError.DISABLED_OPCODE);
                    }

                    if (fExec && 0 <= op.Code && op.Code <= Opcode.OP_PUSHDATA4) 
                    {
                        if (fRequireMinimal && !CheckMinimalPush(ref op)) 
                        {
                            return SetError(out error, ScriptError.MINIMALDATA);
                        }
                        stack.Push(op.Data);
                        // ( -- value)
                    } 
                    else if (fExec || Opcode.OP_IF <= op.Code && op.Code <= Opcode.OP_ENDIF) 
                    {
                        switch (op.Code) {
                            //
                            // Push value
                            //
                            case Opcode.OP_1NEGATE:
                            case Opcode.OP_1:
                            case Opcode.OP_2:
                            case Opcode.OP_3:
                            case Opcode.OP_4:
                            case Opcode.OP_5:
                            case Opcode.OP_6:
                            case Opcode.OP_7:
                            case Opcode.OP_8:
                            case Opcode.OP_9:
                            case Opcode.OP_10:
                            case Opcode.OP_11:
                            case Opcode.OP_12:
                            case Opcode.OP_13:
                            case Opcode.OP_14:
                            case Opcode.OP_15:
                            case Opcode.OP_16:
                            {
                                var sn = new ScriptNum((int)op.Code - (int)Opcode.OP_1 + 1);
                                stack.Push(sn.ToValType());
                                // ( -- value)
                            }
                            break;

                            //
                            // Control
                            //
                            case Opcode.OP_NOP:
                                break;
                            case Opcode.OP_CHECKLOCKTIMEVERIFY:
                                break;
                            case Opcode.OP_CHECKSEQUENCEVERIFY:
                                break;

                            case Opcode.OP_NOP1:
                            case Opcode.OP_NOP4:
                            case Opcode.OP_NOP5:
                            case Opcode.OP_NOP6:
                            case Opcode.OP_NOP7:
                            case Opcode.OP_NOP8:
                            case Opcode.OP_NOP9:
                            case Opcode.OP_NOP10: 
                                {
                                    if ((flags & ScriptFlags.VERIFY_DISCOURAGE_UPGRADABLE_NOPS) != 0) 
                                    {
                                        return SetError(out error, ScriptError.DISCOURAGE_UPGRADABLE_NOPS);
                                    }
                                }
                                break;

                            case Opcode.OP_IF:
                            case Opcode.OP_NOTIF: 
                                {
                                    // <expression> if [statements] [else [statements]]
                                    // endif
                                    var fValue = false;
                                    if (fExec) 
                                    {
                                        if (stack.Count < 1) 
                                        {
                                            return SetError(out error, ScriptError.UNBALANCED_CONDITIONAL);
                                        }
                                        var vch = stack.Pop();
                                        if ((flags & ScriptFlags.VERIFY_MINIMALIF) != 0) 
                                        {
                                            if (vch.Length > 1 || vch.Length == 1 && vch.GetReader().Data.CurrentSpan[0] != 1) 
                                            {
                                                stack.Push(vch);
                                                return SetError(out error, ScriptError.MINIMALIF);
                                            }
                                        }
                                        fValue = vch.ToBool();
                                        if (op.Code == Opcode.OP_NOTIF) 
                                        {
                                            fValue = !fValue;
                                        }
                                    }
                                    vfExec.Push(fValue);
                                }
                                break;

                            case Opcode.OP_ELSE:
                                {
                                    if (vfExec.Count < 1) 
                                    {
                                        return SetError(out error, ScriptError.UNBALANCED_CONDITIONAL);
                                    }
                                    vfExec.Push(!vfExec.Pop());
                                }
                                break;

                            case Opcode.OP_ENDIF: 
                                {
                                    if (vfExec.Count < 1) 
                                    {
                                        return SetError(out error, ScriptError.UNBALANCED_CONDITIONAL);
                                    }
                                    vfExec.Pop();
                                }
                                break;

                            case Opcode.OP_VERIFY: 
                                {
                                    // (true -- ) or
                                    // (false -- false) and return
                                    if (stack.Count < 1) 
                                    {
                                        return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    }
                                    var vch = stack.Pop();
                                    var fValue = vch.ToBool();
                                    if (!fValue) 
                                    {
                                        stack.Push(vch);
                                        return SetError(out error, ScriptError.VERIFY);
                                    }
                                }
                                break;

                            case Opcode.OP_RETURN:
                                return SetError(out error, ScriptError.OP_RETURN);

                            //
                            // Stack ops
                            //
                            case Opcode.OP_TOALTSTACK: 
                                {
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    altStack.Push(stack.Pop());
                                }
                                break;

                            case Opcode.OP_FROMALTSTACK: 
                                {
                                    if (altStack.Count < 1) return SetError(out error, ScriptError.INVALID_ALTSTACK_OPERATION);
                                    stack.Push(altStack.Pop());
                                }
                                break;

                            case Opcode.OP_2DROP:
                                {
                                    // (x1 x2 -- )
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Drop2();
                                }
                                break;

                            case Opcode.OP_2DUP: 
                                {
                                    // (x1 x2 -- x1 x2 x1 x2)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Dup2();
                                }
                                break;

                            case Opcode.OP_3DUP: 
                                {
                                    // (x1 x2 x3 -- x1 x2 x3 x1 x2 x3)
                                    if (stack.Count < 3) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Dup3();
                                }
                                break;

                            case Opcode.OP_2OVER: 
                                {
                                    // (x1 x2 x3 x4 -- x1 x2 x3 x4 x1 x2)
                                    if (stack.Count < 4) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Over2();
                                }
                                break;

                            case Opcode.OP_2ROT: 
                                {
                                    // (x1 x2 x3 x4 x5 x6 -- x3 x4 x5 x6 x1 x2)
                                    if (stack.Count < 6) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Rot2();
                                }
                                break;

                            case Opcode.OP_2SWAP: 
                                {
                                    // (x1 x2 x3 x4 -- x3 x4 x1 x2)
                                    if (stack.Count < 4) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Swap2();
                                }
                                break;

                            case Opcode.OP_IFDUP: 
                                {
                                    // (x - 0 | x x)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var vch = stack.Peek();
                                    if (vch.ToBool())
                                        stack.Push(vch);
                                }
                                break;

                            case Opcode.OP_DEPTH: 
                                {
                                    // -- stacksize
                                    stack.Push(new ScriptNum(stack.Count).ToValType());
                                }
                                break;

                            case Opcode.OP_DROP: 
                                {
                                    // (x -- )
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Pop();
                                }
                                break;

                            case Opcode.OP_DUP: 
                                {
                                    // (x -- x x)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Push(stack.Peek());
                                }
                                break;

                            case Opcode.OP_NIP: 
                                {
                                    // (x1 x2 -- x2)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Nip();
                                }
                                break;

                            case Opcode.OP_OVER: 
                                {
                                    // (x1 x2 -- x1 x2 x1)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Over();
                                }
                                break;

                            case Opcode.OP_PICK:
                            case Opcode.OP_ROLL: 
                                {
                                    // (xn ... x2 x1 x0 n - xn ... x2 x1 x0 xn)
                                    // (xn ... x2 x1 x0 n - ... x2 x1 x0 xn)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var n = stack.Pop().ToScriptNum(fRequireMinimal).GetInt();
                                    if (n < 0 || n >= stack.Count) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    if (op.Code == Opcode.OP_ROLL)
                                        stack.Roll(n);
                                    else
                                        stack.Pick(n);
                                }
                                break;

                            case Opcode.OP_ROT: 
                                {
                                    // (x1 x2 x3 -- x2 x3 x1)
                                    if (stack.Count < 3) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Rot();
                                }
                                break;

                            case Opcode.OP_SWAP: 
                                {
                                    // (x1 x2 -- x2 x1)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Swap();
                                }
                                break;

                            case Opcode.OP_TUCK: 
                                {
                                    // (x1 x2 -- x2 x1 x2)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Tuck();
                                }
                                break;

                            case Opcode.OP_SIZE: 
                                {
                                    // (in -- in size)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var sn = new ScriptNum(stack.Peek().Length);
                                    stack.Push(sn.ToValType());
                                }
                                break;

                            //
                            // Bitwise logic
                            //
                            case Opcode.OP_AND:
                            case Opcode.OP_OR:
                            case Opcode.OP_XOR: 
                                {
                                    // (x1 x2 - out)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var x2 = stack.Pop();
                                    var x1 = stack.Pop();

                                    // Inputs must be the same size
                                    if (x1.Length != x2.Length) return SetError(out error, ScriptError.INVALID_OPERAND_SIZE);

                                    // To avoid allocating, we modify vch1 in place.
                                    switch (op.Code)
                                    {
                                        case Opcode.OP_AND:
                                            stack.Push(x1.BitAnd(x2));
                                            break;

                                        case Opcode.OP_OR:
                                            stack.Push(x1.BitOr(x2));
                                            break;

                                        case Opcode.OP_XOR:
                                            stack.Push(x1.BitXor(x2));
                                            break;
                                    }
                                }
                                break;

                            case Opcode.OP_INVERT:
                                {
                                    // (x -- out)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    stack.Push(stack.Pop().BitInvert());
                                }
                                break;

                            case Opcode.OP_LSHIFT:
                            case Opcode.OP_RSHIFT:
                                {
                                    // (x n -- out)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var nvt = stack.Pop();
                                    var n = nvt.ToInt32();
                                    if (n < 0) {
                                        stack.Push(nvt);
                                        return SetError(out error, ScriptError.INVALID_NUMBER_RANGE);
                                    }
                                    var x = stack.Pop();
                                    var r = op.Code == Opcode.OP_LSHIFT ? x.LShift(n) : x.RShift(n);
                                    stack.Push(r);
                                }
                                break;

                            case Opcode.OP_EQUAL:
                            case Opcode.OP_EQUALVERIFY:
                                // case OP_NOTEQUAL: // use OP_NUMNOTEQUAL
                                {
                                    // (x1 x2 - bool)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var x2 = stack.Pop();
                                    var x1 = stack.Pop();

                                    var fEqual = x1.BitEquals(x2); // (vch1 == vch2);
                                    // OP_NOTEQUAL is disabled because it would be too
                                    // easy to say something like n != 1 and have some
                                    // wiseguy pass in 1 with extra zero bytes after it
                                    // (numerically, 0x01 == 0x0001 == 0x000001)
                                    // if (opcode == OP_NOTEQUAL)
                                    //    fEqual = !fEqual;
                                    stack.Push(fEqual ? VchTrue : VchFalse);
                                    if (op.Code == Opcode.OP_EQUALVERIFY) {
                                        if (fEqual)
                                            stack.Pop();
                                        else
                                            return SetError(out error, ScriptError.EQUALVERIFY);
                                    }
                                }
                                break;

                            //
                            // Numeric
                            //
                            case Opcode.OP_1ADD:
                            case Opcode.OP_1SUB:
                            case Opcode.OP_NEGATE:
                            case Opcode.OP_ABS:
                            case Opcode.OP_NOT:
                            case Opcode.OP_0NOTEQUAL:
                                {
                                    // (in -- out)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var bn = stack.Pop().ToScriptNum(fRequireMinimal);
                                    switch (op.Code) {
                                        case Opcode.OP_1ADD:
                                            bn += ScriptNum.One;
                                            break;
                                        case Opcode.OP_1SUB:
                                            bn -= ScriptNum.One;
                                            break;
                                        case Opcode.OP_NEGATE:
                                            bn = -bn;
                                            break;
                                        case Opcode.OP_ABS:
                                            if (bn < ScriptNum.Zero) {
                                                bn = -bn;
                                            }
                                            break;
                                        case Opcode.OP_NOT:
                                            bn = (bn == ScriptNum.Zero);
                                            break;
                                        case Opcode.OP_0NOTEQUAL:
                                            bn = (bn != ScriptNum.Zero);
                                            break;
                                        default:
                                            return SetError(out error, ScriptError.BAD_OPCODE);
                                    }
                                    stack.Push(bn.ToValType());
                                }
                                break;

                            case Opcode.OP_ADD:
                            case Opcode.OP_SUB:
                            case Opcode.OP_MUL:
                            case Opcode.OP_DIV:
                            case Opcode.OP_MOD:
                            case Opcode.OP_BOOLAND:
                            case Opcode.OP_BOOLOR:
                            case Opcode.OP_NUMEQUAL:
                            case Opcode.OP_NUMEQUALVERIFY:
                            case Opcode.OP_NUMNOTEQUAL:
                            case Opcode.OP_LESSTHAN:
                            case Opcode.OP_GREATERTHAN:
                            case Opcode.OP_LESSTHANOREQUAL:
                            case Opcode.OP_GREATERTHANOREQUAL:
                            case Opcode.OP_MIN:
                            case Opcode.OP_MAX:
                                {
                                    // (x1 x2 -- out)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var bn2 = stack.Pop().ToScriptNum(fRequireMinimal);
                                    var bn1 = stack.Pop().ToScriptNum(fRequireMinimal);
                                    var bn = new ScriptNum(0);
                                    switch (op.Code) {
                                        case Opcode.OP_ADD:
                                            bn = bn1 + bn2;
                                            break;

                                        case Opcode.OP_SUB:
                                            bn = bn1 - bn2;
                                            break;

                                        case Opcode.OP_MUL:
                                            bn = bn1 * bn2;
                                            break;

                                        case Opcode.OP_DIV:
                                            // denominator must not be 0
                                            if (bn2 == 0) return SetError(out error, ScriptError.DIV_BY_ZERO);
                                            bn = bn1 / bn2;
                                            break;

                                        case Opcode.OP_MOD:
                                            // divisor must not be 0
                                            if (bn2 == 0) return SetError(out error, ScriptError.MOD_BY_ZERO);
                                            bn = bn1 % bn2;
                                            break;

                                        case Opcode.OP_BOOLAND:
                                            bn = (bn1 != ScriptNum.Zero && bn2 != ScriptNum.Zero);
                                            break;

                                        case Opcode.OP_BOOLOR:
                                            bn = (bn1 != ScriptNum.Zero || bn2 != ScriptNum.Zero);
                                            break;

                                        case Opcode.OP_NUMEQUAL:
                                            bn = (bn1 == bn2);
                                            break;

                                        case Opcode.OP_NUMEQUALVERIFY:
                                            bn = (bn1 == bn2);
                                            break;

                                        case Opcode.OP_NUMNOTEQUAL:
                                            bn = (bn1 != bn2);
                                            break;

                                        case Opcode.OP_LESSTHAN:
                                            bn = (bn1 < bn2);
                                            break;

                                        case Opcode.OP_GREATERTHAN:
                                            bn = (bn1 > bn2);
                                            break;

                                        case Opcode.OP_LESSTHANOREQUAL:
                                            bn = (bn1 <= bn2);
                                            break;

                                        case Opcode.OP_GREATERTHANOREQUAL:
                                            bn = (bn1 >= bn2);
                                            break;

                                        case Opcode.OP_MIN:
                                            bn = (bn1 < bn2 ? bn1 : bn2);
                                            break;

                                        case Opcode.OP_MAX:
                                            bn = (bn1 > bn2 ? bn1 : bn2);
                                            break;

                                        default:
                                            return SetError(out error, ScriptError.BAD_OPCODE);
                                    }
                                    stack.Push(bn.ToValType());

                                    if (op.Code == Opcode.OP_NUMEQUALVERIFY)
                                    {
                                        var vch = stack.Pop();
                                        if (!vch.ToBool()) 
                                        {
                                            stack.Push(vch);
                                            return SetError(out error, ScriptError.NUMEQUALVERIFY);
                                        }
                                    }
                                }
                                break;

                            case Opcode.OP_WITHIN: 
                                {
                                    // (x1 min2 max3 -- out)
                                    if (stack.Count < 3) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var bn3 = stack.Pop().ToScriptNum(fRequireMinimal);
                                    var bn2 = stack.Pop().ToScriptNum(fRequireMinimal);
                                    var bn1 = stack.Pop().ToScriptNum(fRequireMinimal);
                                    var bn = new ScriptNum(0);
                                    var fValue = (bn2 <= bn1 && bn1 < bn3);
                                    stack.Push(fValue ? VchTrue : VchFalse);
                                }
                                break;

                            //
                            // Crypto
                            //
                            case Opcode.OP_RIPEMD160:
                            case Opcode.OP_SHA1:
                            case Opcode.OP_SHA256:
                            case Opcode.OP_HASH160:
                            case Opcode.OP_HASH256: 
                                {
                                    // (in -- hash)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var vch = stack.Pop();
                                    var data = (byte[])null;
                                    switch (op.Code) 
                                    {
                                        case Opcode.OP_SHA1:
                                            data = new byte[20];
                                            vch.Sequence.Sha1(data);
                                            break;

                                        case Opcode.OP_RIPEMD160:
                                            data = new byte[20];
                                            vch.Sequence.Ripemd160(data);
                                            break;

                                        case Opcode.OP_HASH160:
                                            data = new byte[20];
                                            vch.Sequence.Hash160(data);
                                            break;

                                        case Opcode.OP_SHA256:
                                            data = new byte[32];
                                            vch.Sequence.Sha256(data);
                                            break;

                                        case Opcode.OP_HASH256:
                                            data = new byte[32];
                                            vch.Sequence.Hash256(data);
                                            break;

                                        default:
                                            return SetError(out error, ScriptError.BAD_OPCODE);
                                    }
                                    stack.Push(new VarType(data));
                                }
                                break;

                            case Opcode.OP_CODESEPARATOR: 
                                {
                                    // Hash starts after the code separator
                                    beginCodeHash = ros.Data.Start;
                                }
                                break;

                            case Opcode.OP_CHECKSIG:
                            case Opcode.OP_CHECKSIGVERIFY:
                                {
                                    // (sig pubkey -- bool)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);
                                    var vchPubKey = stack.Pop();
                                    var vchSig = stack.Pop();

                                    if (!CheckSignatureEncoding(vchSig, flags, ref error) ||
                                        !CheckPubKeyEncoding(vchPubKey, flags, ref error)) {
                                        // error is set
                                        return false;
                                    }

                                    // Subset of script starting at the most recent codeseparator
                                    var scriptCode = script.Slice(beginCodeHash, pend);

                                    // Remove signature for pre-fork scripts
                                    CleanupScriptCode(scriptCode, vchSig, flags);

                                    bool fSuccess = checker.CheckSignature(vchSig, vchPubKey, scriptCode, flags);

                                    if (!fSuccess && (flags & ScriptFlags.VERIFY_NULLFAIL) != 0 && vchSig.Length > 0) {
                                        return SetError(out error, ScriptError.SIG_NULLFAIL);
                                    }

                                    stack.Push(fSuccess ? VchTrue : VchFalse);
                                    if (op.Code == Opcode.OP_CHECKSIGVERIFY) {
                                        if (fSuccess) {
                                            stack.Pop();
                                        } else {
                                            return SetError(out error, ScriptError.CHECKSIGVERIFY);
                                        }
                                    }
                                }
                                break;

                            //
                            // Byte string operations
                            //
                            case Opcode.OP_CAT: {
                                    // (x1 x2 -- out)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);

                                    var x2 = stack.Pop();
                                    var x1 = stack.Pop();
                                    if (x1.Length + x2.Length > RootService.Network.Consensus.MaxScriptElementSize) return SetError(out error, ScriptError.PUSH_SIZE);

                                    stack.Push(x1.Cat(x2));
                                }
                                break;

                            case Opcode.OP_SPLIT: {
                                    // (data position -- x1 x2)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);

                                    var position = stack.Pop().ToScriptNum(fRequireMinimal).GetInt();
                                    var data = stack.Pop();

                                    // Make sure the split point is apropriate.
                                    if (position < 0 || position > data.Length)
                                        return SetError(out error, ScriptError.INVALID_SPLIT_RANGE);

                                    var (x1, x2) = data.Split(position);
                                    stack.Push(x1);
                                    stack.Push(x2);
                                }
                                break;

                            //
                            // Conversion operations
                            //
                            case Opcode.OP_NUM2BIN: {
                                    // (in size -- out)
                                    if (stack.Count < 2) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);

                                    var size = stack.Pop().ToScriptNum(fRequireMinimal).GetInt();
                                    if (size < 0 || size > RootService.Network.Consensus.MaxScriptElementSize)
                                        return SetError(out error, ScriptError.PUSH_SIZE);

                                    var num = stack.Pop();

                                    var (bin, ok) = num.Num2Bin((uint)size);

                                    if (!ok) return SetError(out error, ScriptError.IMPOSSIBLE_ENCODING);

                                    stack.Push(bin);
                                }
                                break;

                            case Opcode.OP_BIN2NUM: {
                                    // (in -- out)
                                    if (stack.Count < 1) return SetError(out error, ScriptError.INVALID_STACK_OPERATION);

                                    var bin = stack.Pop();

                                    var (num, ok) = bin.Bin2Num();

                                    if (!ok) return SetError(out error, ScriptError.INVALID_NUMBER_RANGE);

                                    stack.Push(num);
                                }
                                break;

                            default:
                                return SetError(out error, ScriptError.BAD_OPCODE);
                        }
                    }

                    if (stack.Count + altStack.Count > 1000) return SetError(out error, ScriptError.STACK_SIZE);
                }
            }
            catch (ScriptNum.OverflowError) 
            {
                return SetError(out error, ScriptError.SCRIPTNUM_OVERFLOW);
            }
            catch (ScriptNum.MinEncodeError) 
            {
                return SetError(out error, ScriptError.SCRIPTNUM_MINENCODE);
            }
            catch 
            {
                return SetError(out error, ScriptError.UNKNOWN_ERROR);
            }

            return vfExec.Count != 0 
                ? SetError(out error, ScriptError.UNBALANCED_CONDITIONAL) 
                : SetSuccess(out error);
        }
    }
}

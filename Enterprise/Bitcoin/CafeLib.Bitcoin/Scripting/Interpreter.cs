#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using System.Numerics;
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
    public class Interpreter
    {
        private Script _script;
        private Transaction _tx;
        private int _inIndex;
        private SequencePosition _pc;
        private SequencePosition _pBeginCodeHash;
        private SequencePosition _pcEnd;
        private ScriptStack<bool> _executionStack;
        private ScriptStack<VarType>  _altStack;
        private ScriptStack<bool> _ifStack;
        private ScriptFlags _flags;
        private string _errStr;
        private int _opCount;
        private UInt256 _value;
        private ScriptError _error;

        private const ScriptFlags DefaultFlags = ScriptFlags.VERIFY_P2SH | ScriptFlags.VERIFY_CHECKLOCKTIMEVERIFY;
        private const int LocktimeThreshold = 500000000;  // Tue Nov  5 00:53:20 1985 UTC


        public Interpreter(Script script, Transaction tx, int inIndex)
        {
            _script = script;
            _tx = tx;
            _inIndex = inIndex;

            _executionStack = new ScriptStack<bool>();
            _altStack = new ScriptStack<VarType>();
            _ifStack = new ScriptStack<bool>();

            _pc = script.Data.Sequence.Data.Start;
            _pBeginCodeHash = _pc;
            _pcEnd = script.Data.Sequence.Data.End;

            _errStr = "";
            _flags = DefaultFlags;
            _opCount = 0;
            _value = new UInt256(0);
        }

        private bool CheckSigEncoding(VarType vchSig)
        {
            // Empty signature. Not strictly DER encoded, but allowed to provide a
            // compact way to provide an invalid signature for use with CHECK(MULTI)SIG
            if (vchSig.IsEmpty)
            {
                return true;
            }
            if ((_flags &
                 (ScriptFlags.VERIFY_DERSIG |
                  ScriptFlags.VERIFY_LOW_S |
                  ScriptFlags.VERIFY_STRICTENC)) != 0
                && !Signature.IsTxDer(vchSig))
            {
                _errStr = "SCRIPT_ERR_SIG_DER";
                return false;
            }
            else if ((_flags & ScriptFlags.VERIFY_LOW_S) != 0)
            {
                if (!IsLowDerSignature(vchSig, ref _error))
                {
                    return false;
                }
            }
            else if ((_flags & ScriptFlags.VERIFY_STRICTENC) != 0)
            {
                //let sig = new Sig().fromTxFormat(buf)
                //if (!sig.hasDefinedHashType())
                //{
                //    this.errStr = 'SCRIPT_ERR_SIG_HASHTYPE'
                //    return false
                //}
            }

            return true;
        }

        private bool CheckPubKeyEncoding()
        {
            return false;
        }

        private bool CheckLockTime(int nLockTime)
        {
            // There are two kinds of nLockTime: lock-by-blockheight
            // and lock-by-blocktime, distinguished by whether
            // nLockTime < LOCKTIME_THRESHOLD.
            //
            // We want to compare apples to apples, so fail the script
            // unless the type of nLockTime being tested is the same as
            // the nLockTime in the transaction.
            if (
                !(
                    _tx.LockTime < LocktimeThreshold && nLockTime < LocktimeThreshold ||
                    _tx.LockTime >= LocktimeThreshold && nLockTime >= LocktimeThreshold
                )
            )
            {
                return false;
            }

            // Now that we know we're comparing apples-to-apples, the
            // comparison is a simple numeric one.
            if (nLockTime > _tx.LockTime)
            {
                return false;
            }

            // Finally the nLockTime feature can be disabled and thus
            // CHECKLOCKTIMEVERIFY bypassed if every txIn has been
            // finalized by setting nSequence to maxint. The
            // transaction would be allowed into the blockchain, making
            // the opCode ineffective.
            //
            // Testing if this vin is not final is sufficient to
            // prevent this condition. Alternatively we could test all
            // inputs, but testing just this input minimizes the data
            // required to prove correct CHECKLOCKTIMEVERIFY execution.
            return TxIn.SequenceFinal != _tx.Inputs[_inIndex].Sequence;
        }

        private static bool IsLowDerSignature(VarType vchSig, ref ScriptError error)
        {
            //if (!IsValidSignatureEncoding(vchSig)) return SetError(out error, ScriptError.SIG_DER);

            var sigInput = vchSig.Slice(0, (int)vchSig.Length - 1);

            //if (!PublicKey.CheckLowS(sigInput)) return SetError(out error, ScriptError.SIG_HIGH_S);

            return true;
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

        private static SignatureHashType GetHashType(VarType vchSig)
        {
            return new SignatureHashType(vchSig.Length == 0 ? SignatureHashEnum.Unsupported : (SignatureHashEnum)vchSig.LastByte);
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

            ReadOnlyByteSpan sig = vchSig;

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
            if (lenS > 1 && (sig[lenR + 6] == 0x00) && (sig[lenR + 7] & 0x80) == 0)
            {
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

    }
}
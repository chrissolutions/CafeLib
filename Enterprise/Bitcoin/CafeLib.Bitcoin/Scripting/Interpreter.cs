#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
            return true;
        }

        private static bool IsLowDerSignature(VarType vchSig, ref ScriptError error)
        {
            //if (!IsValidSignatureEncoding(vchSig)) return SetError(out error, ScriptError.SIG_DER);

            var sigInput = vchSig.Slice(0, (int)vchSig.Length - 1);

            //if (!PublicKey.CheckLowS(sigInput)) return SetError(out error, ScriptError.SIG_HIGH_S);

            return true;
        }
    }
}
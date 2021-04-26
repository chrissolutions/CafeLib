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
        private BigInteger _value;

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
            _value = new BigInteger(0);
        }


        private bool CheckSigEncoding(VarType vchSig)
        {
            return false;
        }

        private bool CheckPubKeyEncoding()
        {
            return false;
        }

        private bool CheckLockTime(int nLockTime)
        {
            return true;
        }
    }
}
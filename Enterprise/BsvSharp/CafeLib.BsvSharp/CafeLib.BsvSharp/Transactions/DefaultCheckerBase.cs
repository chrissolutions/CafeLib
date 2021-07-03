#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Transactions
{
    public class DefaultCheckerBase : ISignatureChecker
    {
        public virtual bool CheckSignature(VarType scriptSig, VarType vchPubKey, Script script, ScriptFlags flags) => false;

        public bool CheckLockTime(uint lockTime) => false;

        public bool CheckSequence(uint sequenceNumber) => false;
    }
}

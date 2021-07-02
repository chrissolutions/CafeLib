#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Transactions
{
    public interface ISignatureChecker
    {
        bool CheckSignature(VarType scriptSig, VarType vchPubKey, Script script, ScriptFlags flags);

        bool CheckLockTime(ScriptNum nLockTime);

        bool CheckSequence(ScriptNum nSequence);
    }
}

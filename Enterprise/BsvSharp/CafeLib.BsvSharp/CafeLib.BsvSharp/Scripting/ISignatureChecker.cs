#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Scripting
{
    public interface ISignatureChecker 
    {
        bool CheckSignature(VarType scriptSig, VarType vchPubKey, Script script, ScriptFlags flags) => false;

        bool CheckLockTime(ScriptNum nLockTime) => false;

        bool CheckSequence(ScriptNum nSequence) => false;
    }
}

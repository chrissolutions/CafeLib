#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion
namespace CafeLib.Bitcoin.Scripting
{
    public class SignatureCheckerBase
    {
        public virtual bool CheckSignature(ValType scriptSig, ValType vchPubKey, Script script, ScriptFlags flags) => false;

        public virtual bool CheckLockTime(ScriptNum nLockTime) => false;

        public virtual bool CheckSequence(ScriptNum nSequence) => false;
    }
}

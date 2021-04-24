﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Scripting
{
    public class TransactionSignatureChecker : SignatureCheckerBase
    {
        public override bool CheckSignature(ValType scriptSig, ValType vchPubKey, Script script, ScriptFlags flags)
        {
            const std::vector<uint8_t> &vchSigIn, const std::vector<uint8_t> &vchPubKey,
            const CScript &scriptCode, bool enabledSighashForkid) const {
                CPubKey pubkey(vchPubKey);
                if (!pubkey.IsValid())
                {
                    return false;
                }

                // Hash type is one byte tacked on to the end of the signature
                std::vector<uint8_t> vchSig(vchSigIn);
                if (vchSig.empty())
                {
                    return false;
                }
                SigHashType sigHashType = GetHashType(vchSig);
                vchSig.pop_back();

                uint256 sighash = SignatureHash(scriptCode, *txTo, nIn, sigHashType, amount,
                    this->txdata, enabledSighashForkid);

                if (!VerifySignature(vchSig, pubkey, sighash))
                {
                    return false;
                }

                return true;
            }

        public virtual bool CheckLockTime(ScriptNum nLockTime) => false;

        public virtual bool CheckSequence(ScriptNum nSequence) => false;
    }
}

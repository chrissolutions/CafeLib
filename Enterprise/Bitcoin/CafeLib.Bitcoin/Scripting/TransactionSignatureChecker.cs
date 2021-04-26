#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Scripting
{
    public class TransactionSignatureChecker : SignatureCheckerBase
    {
        private Script _script;
        private Transaction _tx;
        private int _index;

        private const int LocktimeThreshold = 500000000;  // Tue Nov  5 00:53:20 1985 UTC

        public TransactionSignatureChecker(Script script, Transaction tx, int index)
        {
            _script = script;
            _tx = tx;
            _index = index;
        }

        public override bool CheckSignature(VarType scriptSig, VarType vchPubKey, Script script, ScriptFlags flags)
        {
            var publicKey = new PublicKey(vchPubKey);
            if (!publicKey.IsValid) return false;
            if (vchPubKey == VarType.Empty) return false;
            var sigHashType = new SignatureHashType(scriptSig.LastByte);

            return false;

            //    const std::vector<uint8_t> &vchSigIn, const std::vector<uint8_t> &vchPubKey,
            //    const CScript &scriptCode, bool enabledSighashForkid) const
            //    {
            //        CPubKey pubkey(vchPubKey);
            //        if (!pubkey.IsValid())
            //        {
            //            return false;
            //        }

            //        // Hash type is one byte tacked on to the end of the signature
            //        std::vector<uint8_t> vchSig(vchSigIn);
            //        if (vchSig.empty())
            //        {
            //            return false;
            //        }
            //        SigHashType sigHashType = GetHashType(vchSig);
            //        vchSig.pop_back();

            //        uint256 sighash = SignatureHash(scriptCode, *txTo, nIn, sigHashType, amount,
            //            this->txdata, enabledSighashForkid);

            //        if (!VerifySignature(vchSig, pubkey, sighash))
            //        {
            //            return false;
            //        }

            //        return true;
            //    }
        }

        public bool CheckLockTime(int nLockTime)
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
            // finalized by setting nSequence to max int. The
            // transaction would be allowed into the blockchain, making
            // the opCode ineffective.
            //
            // Testing if this vin is not final is sufficient to
            // prevent this condition. Alternatively we could test all
            // inputs, but testing just this input minimizes the data
            // required to prove correct CHECKLOCKTIMEVERIFY execution.
            return TxIn.SequenceFinal != _tx.Inputs[_index].Sequence;
        }

        public override bool CheckSequence(ScriptNum nSequence) => false;
    }
}

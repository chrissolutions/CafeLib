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
        private readonly Transaction _tx;
        private readonly int _index;

        private const int LocktimeThreshold = 500000000; // Tue Nov  5 00:53:20 1985 UTC

        public TransactionSignatureChecker(Script script, Transaction tx, int index)
        {
            _script = script;
            _tx = tx;
            _index = index;
        }

        public override bool CheckSignature(VarType scriptSig, VarType vchPubKey, Script script, ScriptFlags flags)
        {
            var signature = new Signature(scriptSig);

            var fSuccess = false;
            try
            {
                //var publicKey = new PublicKey(vchPubKey);
                //if (!publicKey.IsValid) return false;
                //if (vchPubKey == VarType.Empty) return false;
                //var sig = new Signature(scriptSig);
                //fSuccess = this.tx.verify(
                //    sig,
                //    pubKey,
                //    this.nIn,
                //    subScript,
                //    Boolean(this.flags & Interp.SCRIPT_VERIFY_LOW_S),
                //    this.valueBn,
                //    this.flags
                //)
            }
            catch
            {
                // invalid sig or pubKey
                fSuccess = false;
            }











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

        /// <summary>
        /// Translated from bitcoin core's CheckSequence.
        /// </summary>
        /// <param name="nSequence"></param>
        /// <returns></returns>
        public override bool CheckSequence(ScriptNum nSequence)
        {
            // Relative lock times are supported by comparing the passed
            // in operand to the sequence number of the input.
            var txToSequence = _tx.Inputs[_index].Sequence;

            // Fail if the transaction's version number is not set high
            // enough to trigger Bip 68 rules.
            if (_tx.Version < 2)
            {
                return false;
            }

            // Sequence numbers with their most significant bit set are not
            // consensus constrained. Testing that the transaction's sequence
            // number do not have this bit set prevents using this property
            // to get around a CHECKSEQUENCEVERIFY check.
            if ((txToSequence & TxIn.SequenceLocktimeDisableFlag) != 0)
            {
                return false;
            }

            // Mask off any bits that do not have consensus-enforced meaning
            // before doing the integer comparisons
            const uint nLockTimeMask = TxIn.SequenceLocktimeTypeFlag | TxIn.SequenceLocktimeMask;
            var txToSequenceMasked = txToSequence & nLockTimeMask;
            var nSequenceMasked = nSequence.GetInt() & nLockTimeMask;

            // There are two kinds of nSequence: lock-by-blockheight
            // and lock-by-blocktime, distinguished by whether
            // nSequenceMasked < CTxIn::SEQUENCE_LOCKTIME_TYPE_FLAG.
            //
            // We want to compare apples to apples, so fail the script
            // unless the type of nSequenceMasked being tested is the same as
            // the nSequenceMasked in the transaction.
            if (
                !(
                    txToSequenceMasked < TxIn.SequenceLocktimeTypeFlag &&
                    nSequenceMasked < TxIn.SequenceLocktimeTypeFlag ||
                    txToSequenceMasked >= TxIn.SequenceLocktimeTypeFlag &&
                    nSequenceMasked >= TxIn.SequenceLocktimeTypeFlag
                )
            )
            {
                return false;
            }

            // Now that we know we're comparing apples-to-apples, the
            // comparison is a simple numeric one.
            return nSequenceMasked <= txToSequenceMasked;
        }


        public bool VerifyTransaction
        (
            VarType sig,
            VarType pubKey,
            int nIn,
            Script subScript,
            UInt256 value,
            bool enforceLowS = false,
            uint flags = (uint) ScriptFlags.ENABLE_SIGHASH_FORKID
        )
        {
            return false;
        }


        //private UInt256 SigHash(
        //    SignatureHashType hashType,
        //    int nin,
        //    Script subScript,
        //    UInt256 value,
        //    uint flags
        //)
        //{
        //    // start with UAHF part (Bitcoin SV)
        //    // https://github.com/Bitcoin-UAHF/spec/blob/master/replay-protected-sighash.md

        //    if ((hashType.RawSigHashType & (uint) SignatureHashEnum.ForkId) != 0U &&
        //        (flags & (uint) ScriptFlags.ENABLE_SIGHASH_FORKID) != 0U)
        //    {
        //        let hashPrevouts = Buffer.alloc(32, 0)
        //        let hashSequence = Buffer.alloc(32, 0)
        //        let hashOutputs = Buffer.alloc(32, 0)

        //    }
        //}


        //private UInt256 HashPrevOuts()
        //{
        //    foreach (var txIn in _tx.Inputs)
        //    {
        //        txIn
        //    }



        //        const bw = new Bw()
        //        for (const i in this.txIns) {
        //            const txIn = this.txIns[i]
        //            bw.write(txIn.txHashBuf) // outpoint (1/2)
        //            bw.writeUInt32LE(txIn.txOutNum) // outpoint (2/2)
        //        }
        //        return Hash.sha256Sha256(bw.toBuffer())
        //    }

        //}

    }
}

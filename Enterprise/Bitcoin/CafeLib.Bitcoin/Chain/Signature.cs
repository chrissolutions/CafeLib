using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;
using CafeLib.Bitcoin.Scripting;
using CafeLib.Bitcoin.Units;

// ReSharper disable InconsistentNaming

namespace CafeLib.Bitcoin.Chain
{
    public class Signature
    {
        private UInt256 _r;
        private UInt256 _s;
        private SignatureHashType _hashType;

        public Signature()
        {
            _r = UInt256.One;
            _s = UInt256.One;
            _hashType = new SignatureHashType(SignatureHashEnum.All);
        }

        public Signature(UInt256 r, UInt256 s, SignatureHashType hashType)
        {
            _r = r;
            _s = s;
            _hashType = hashType;
        }

        public Signature(VarType signature)
            : this()
        {
            if (!signature.IsEmpty)
            {
                var hashType = new SignatureHashType(signature.LastByte);
                var derSig = signature.Slice(0, (int)signature.Length - 1);
                var (r, s) = ParseDer(derSig);
                _r = r;
                _s = s;
                _hashType = hashType;
            }
        }

        /// <summary>
        /// This function is translated from bitcoind's IsDERSignature and is used in
        /// the script interpreter.This "DER" format actually includes an extra byte,
        /// the nHashType, at the end.It is really the tx format, not DER format.
        /// 
        /// A canonical signature exists of: [30] [total len] [02] [len R] [R] [02] [len S] [S] [hashtype]
        /// Where R and S are not negative (their first byte has its highest bit not set), and not
        /// excessively padded(do not start with a 0 byte, unless an otherwise negative number follows,
        /// in which case a single 0 byte is necessary and even required).
        /// 
        /// See https://bitcointalk.org/index.php?topic=8392.msg127623#msg127623
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool IsTxDer(VarType signature)
        {
            ReadOnlyByteMemory buf = signature;

            if (buf.Length < 9)
            {
                //  Non-canonical signature: too short
                return false;
            }
            if (buf.Length > 73)
            {
                // Non-canonical signature: too long
                return false;
            }
            if (buf[0] != 0x30)
            {
                //  Non-canonical signature: wrong type
                return false;
            }
            if (buf[1] != buf.Length - 3)
            {
                //  Non-canonical signature: wrong length marker
                return false;
            }

            var nLEnR = buf[3];
            if (5 + nLEnR >= buf.Length)
            {
                //  Non-canonical signature: S length misplaced
                return false;
            }

            var nLEnS = buf[5 + nLEnR];
            if (nLEnR + nLEnS + 7 != buf.Length)
            {
                //  Non-canonical signature: R+S length mismatch
                return false;
            }

            var R = buf.Slice(4);
            if (buf[4 - 2] != 0x02)
            {
                //  Non-canonical signature: R value type mismatch
                return false;
            }
            if (nLEnR == 0)
            {
                //  Non-canonical signature: R length is zero
                return false;
            }
            if ((R[0] & 0x80) != 0)
            {
                //  Non-canonical signature: R value negative
                return false;
            }
            if (nLEnR > 1 && R[0] == 0x00 && (R[1] & 0x80) != 0)
            {
                //  Non-canonical signature: R value excessively padded
                return false;
            }

            var S = buf.Slice(6 + nLEnR);
            if (buf[6 + nLEnR - 2] != 0x02)
            {
                //  Non-canonical signature: S value type mismatch
                return false;
            }
            if (nLEnS == 0)
            {
                //  Non-canonical signature: S length is zero
                return false;
            }
            if ((S[0] & 0x80) != 0)
            {
                //  Non-canonical signature: S value negative
                return false;
            }
            if (nLEnS > 1 && S[0] == 0x00 && (S[1] & 0x80) != 0)
            {
                //  Non-canonical signature: S value excessively padded
                return false;
            }

            return true;
        }

        public bool Verify
        (
            VarType sig,
            VarType pubKey,
            int index,
            VarType subScript,
            Amount amount,
            bool enforceLowS = false,
            ScriptFlags flags = ScriptFlags.ENABLE_SIGHASH_FORKID,
            object hashCache = null
        )
        {
            //var hashBuf = ComputeSignatureHash(sig.nHashType, index, subScript, amount, flags, hashCache);
            //return Ecdsa.verify(hashBuf, sig, pubKey, 'little', enforceLowS)

            return true;
        }

        public static UInt256 ComputeSignatureHash
        (
            Script scriptCode,
            Transaction txTo,
            int nIn,
            SignatureHashType sigHashType,
            Amount amount,
            ScriptFlags flags = ScriptFlags.ENABLE_SIGHASH_FORKID
        )
        {
            if (sigHashType.HasForkId && (flags & ScriptFlags.ENABLE_SIGHASH_FORKID) != 0)
            {
                var hashPrevOuts = new UInt256();
                var hashSequence = new UInt256();
                var hashOutputs = new UInt256();

                if (!sigHashType.HasAnyoneCanPay)
                {
                    hashPrevOuts = GetPrevOutHash(txTo);
                }

                var baseNotSingleOrNone =
                    (sigHashType.GetBaseType() != BaseSignatureHashEnum.Single) &&
                    (sigHashType.GetBaseType() != BaseSignatureHashEnum.None);

                if (!sigHashType.HasAnyoneCanPay && baseNotSingleOrNone)
                {
                    hashSequence = GetSequenceHash(txTo);
                }

                if (baseNotSingleOrNone)
                {
                    hashOutputs = GetOutputsHash(txTo);
                }
                else if (sigHashType.GetBaseType() == BaseSignatureHashEnum.Single && nIn < txTo.Outputs.Length)
                {
                    using var hw = new HashWriter();
                    hw.Add(txTo.Outputs[nIn]);
                    hashOutputs = hw.GetHashFinal();
                }

                using var writer = new HashWriter();
                writer
                    // Version
                    .Add(txTo.Version)
                    // Input prevouts/nSequence (none/all, depending on flags)
                    .Add(hashPrevOuts)
                    .Add(hashSequence)
                    // The input being signed (replacing the scriptSig with scriptCode +
                    // amount). The prevout may already be contained in hashPrevout, and the
                    // nSequence may already be contain in hashSequence.
                    .Add(txTo.Inputs[nIn].PrevOut)
                    .Add(scriptCode)
                    .Add(amount.Satoshis)
                    .Add(txTo.Inputs[nIn].Sequence)
                    // Outputs (none/one/all, depending on flags)
                    .Add(hashOutputs)
                    // Locktime
                    .Add(txTo.LockTime)
                    // Sighash type
                    .Add(sigHashType.RawSigHashType)
                    ;
                return writer.GetHashFinal();
            }

            if (nIn >= txTo.Inputs.Length)
            {
                //  nIn out of range
                return UInt256.One;
            }

            // Check for invalid use of SIGHASH_SINGLE
            if (sigHashType.GetBaseType() == BaseSignatureHashEnum.Single && nIn >= txTo.Outputs.Length)
            {
                //  nOut out of range
                return UInt256.One;
            }

            {
                // Original digest algorithm...
                var hasAnyoneCanPay = sigHashType.HasAnyoneCanPay;
                var numberOfInputs = hasAnyoneCanPay ? 1 : txTo.Inputs.Length;
                using var writer = new HashWriter();
                // Start with the version...
                writer.Add(txTo.Version);
                // Add Input(s)...
                if (hasAnyoneCanPay)
                {
                    // AnyoneCanPay serializes only the input being signed.
                    var i = txTo.Inputs[nIn];
                    writer
                        .Add((byte)1)
                        .Add(i.PrevOut)
                        .Add(scriptCode, true)
                        .Add(i.Sequence);
                }
                else
                {
                    // Non-AnyoneCanPay case. Process all inputs but handle input being signed in its own way.
                    var isSingleOrNone = sigHashType.IsBaseSingle || sigHashType.IsBaseNone;
                    writer.Add(txTo.Inputs.Length.AsVarIntBytes());
                    for (var nInput = 0; nInput < txTo.Inputs.Length; nInput++)
                    {
                        var i = txTo.Inputs[nInput];
                        writer.Add(i.PrevOut);
                        if (nInput != nIn)
                            writer.Add(Script.None);
                        else
                            writer.Add(scriptCode, true);
                        if (nInput != nIn && isSingleOrNone)
                            writer.Add(0);
                        else
                            writer.Add(i.Sequence);
                    }
                }
                // Add Output(s)...
                var nOutputs = sigHashType.IsBaseNone ? 0 : sigHashType.IsBaseSingle ? nIn + 1 : txTo.Outputs.Length;
                writer.Add(nOutputs.AsVarIntBytes());
                for (var nOutput = 0; nOutput < nOutputs; nOutput++)
                {
                    if (sigHashType.IsBaseSingle && nOutput != nIn)
                        writer.Add(TxOut.Null);
                    else
                        writer.Add(txTo.Outputs[nOutput]);
                }
                // Finish up...
                writer
                    .Add(txTo.LockTime)
                    .Add(sigHashType.RawSigHashType)
                    ;
                return writer.GetHashFinal();
            }
        }

        private static UInt256 GetPrevOutHash(Transaction txTo)
        {
            using var hw = new HashWriter();
            foreach (var i in txTo.Inputs)
            {
                hw.Add(i.PrevOut);
            }

            return hw.GetHashFinal();
        }

        private static UInt256 GetSequenceHash(Transaction txTo)
        {
            using var hw = new HashWriter();
            foreach (var i in txTo.Inputs)
            {
                hw.Add(i.Sequence);
            }

            return hw.GetHashFinal();
        }

        private static UInt256 GetOutputsHash(Transaction txTo)
        {
            using var hw = new HashWriter();
            foreach (var o in txTo.Outputs)
            {
                hw.Add(o);
            }

            return hw.GetHashFinal();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="derSig"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        /// <remarks>
        /// In order to mimic the non-strict DER encoding of OpenSSL, set strict = false.
        /// </remarks>
        private static (UInt256 r, UInt256 s) ParseDer(VarType derSig, bool strict = true)
        {
            ReadOnlyByteMemory buffer = derSig;
            var header = buffer[0];
            if (header != 0x30)
            {
                throw new InvalidOperationException("Header byte should be 0x30");
            }

            var length = (int)buffer[1];
            var bufLength = buffer.Slice(2).Length;
            if (strict && length != bufLength)
            {
                throw new InvalidOperationException("Length byte should length of what follows");
            }
            else
            {
                length = length < bufLength ? length : bufLength;
            }

            var rheader = buffer[2 + 0];
            if (rheader != 0x02)
            {
                throw new InvalidOperationException("Integer byte for r should be 0x02");
            }

            int rLength = buffer[2 + 1];
            var rBuffer = buffer.Slice(2 + 2, 2 + 2 + rLength);
            var r = new UInt256(rBuffer);
            //var rNegative = buffer[2 + 1 + 1] == 0x00;
            if (rLength != rBuffer.Length)
            {
                throw new InvalidOperationException("Length of r incorrect");
            }

            var sHeader = buffer[2 + 2 + rLength + 0];
            if (sHeader != 0x02)
            {
                throw new InvalidOperationException("Integer byte for s should be 0x02");
            }

            int sLength = buffer[2 + 2 + rLength + 1];
            var sBuffer = buffer.Slice(2 + 2 + rLength + 2, 2 + 2 + rLength + 2 + sLength);
            var s = new UInt256(sBuffer);
            //var sNegative = buffer[2 + 2 + rLength + 2 + 2] == 0x00;
            if (sLength != sBuffer.Length)
            {
                throw new InvalidOperationException("Length of s incorrect");
            }

            var sumLength = 2 + 2 + rLength + 2 + sLength;
            if (length != sumLength - 2)
            {
                throw new InvalidOperationException("Length of signature incorrect");
            }

            return (r, s);
        }
    }
}

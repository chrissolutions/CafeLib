﻿using System;
using System.Linq;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Scripting;

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
            return true;
        }


        /**
           */
    //    static IsTxDer(buf)
    //    {
    //        if (buf.length < 9)
    //        {
    //            //  Non-canonical signature: too short
    //            return false
    //        }
    //        if (buf.length > 73)
    //        {
    //            // Non-canonical signature: too long
    //            return false
    //        }
    //        if (buf[0] !== 0x30)
    //        {
    //            //  Non-canonical signature: wrong type
    //            return false
    //        }
    //        if (buf[1] !== buf.length - 3)
    //        {
    //            //  Non-canonical signature: wrong length marker
    //            return false
    //        }
    //        const nLEnR = buf[3]
    //      if (5 + nLEnR >= buf.length)
    //        {
    //            //  Non-canonical signature: S length misplaced
    //            return false
    //}
    //        const nLEnS = buf[5 + nLEnR]
    //      if (nLEnR + nLEnS + 7 !== buf.length)
    //        {
    //            //  Non-canonical signature: R+S length mismatch
    //            return false
    //}

    //        const R = buf.slice(4)
    //      if (buf[4 - 2] !== 0x02)
    //        {
    //            //  Non-canonical signature: R value type mismatch
    //            return false
    //}
    //        if (nLEnR === 0)
    //        {
    //            //  Non-canonical signature: R length is zero
    //            return false
    //        }
    //        if (R[0] & 0x80)
    //        {
    //            //  Non-canonical signature: R value negative
    //            return false
    //        }
    //        if (nLEnR > 1 && R[0] === 0x00 && !(R[1] & 0x80))
    //        {
    //            //  Non-canonical signature: R value excessively padded
    //            return false
    //        }

    //        const S = buf.slice(6 + nLEnR)
    //      if (buf[6 + nLEnR - 2] !== 0x02)
    //        {
    //            //  Non-canonical signature: S value type mismatch
    //            return false
    //}
    //        if (nLEnS === 0)
    //        {
    //            //  Non-canonical signature: S length is zero
    //            return false
    //        }
    //        if (S[0] & 0x80)
    //        {
    //            //  Non-canonical signature: S value negative
    //            return false
    //        }
    //        if (nLEnS > 1 && S[0] === 0x00 && !(S[1] & 0x80))
    //        {
    //            //  Non-canonical signature: S value excessively padded
    //            return false
    //        }
    //        return true
    //    }

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

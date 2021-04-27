﻿using System;
using System.Linq;
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

        public static bool IsTxDer(VarType vchSig)
        {
            return true;
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
            byte[] buffer = derSig;
            var header = buffer.First();
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

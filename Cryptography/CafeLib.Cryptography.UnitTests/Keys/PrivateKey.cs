using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using CafeLib.Core.Buffers;
using CafeLib.Core.Numerics;
using CafeLib.Cryptography.BouncyCastle.Security;

namespace CafeLib.Cryptography.UnitTests.Keys
{
    public class PrivateKey : IEquatable<PrivateKey>
    {
        private const int KeySize = UInt256.Length;
        //private static readonly ECKey _ecKey = new ECKey();

        /// <summary>
        /// PrivateKey data.
        /// </summary>
        private readonly UInt256 _keyData;

        /// <summary>
        /// HardenedBit.
        /// </summary>
        private const uint HardenedBit = 0x80000000;

        public PrivateKey()
            : this(true)
        {

        }

        public PrivateKey(bool compressed)
        {
            var data = new byte[KeySize];

            do
            {
                data = Randomizer.GetStrongRandBytes(UInt256.Length);
            } while (!CheckData(data));

            //SetBytes(data, data.Length, fCompressedIn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyData"></param>
        public PrivateKey(UInt256 keyData)
        {
            _keyData = keyData;
            IsCompressed = true;
            IsValid = true;
        }


        /// <summary>
        /// Determines validity of the private key.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// True if the corresponding public key is compressed.
        /// </summary>
        public bool IsCompressed { get; private set; }


        public static implicit operator BigInteger(PrivateKey rhs) => rhs._keyData.ToBigInteger();
        public static implicit operator ReadOnlyByteSpan(PrivateKey rhs) => rhs._keyData.Span;


        public bool Equals(PrivateKey other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PrivateKey) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="compressed"></param>
        private void Set(ReadOnlyByteSpan data, bool compressed = true)
        {
            if (data.Length != UInt256.Length || !VerifyKeyData(data))
                IsValid = false;
            else
            {
                data.CopyTo(_keyData.Span);
                IsCompressed = compressed;
                IsValid = true;
            }
        }

        /// <summary>
        /// Verify key data.
        /// </summary>
        /// <param name="vch"></param>
        /// <returns></returns>
        private static bool VerifyKeyData(ReadOnlyByteSpan vch)
        {
            return true;
            //return Secp256K1.SecretKeyVerify(vch);
        }

        private bool CheckData(ReadOnlyByteSpan data)
        {
            // Do not convert to OpenSSL's data structures for range-checking keys,
            // it's easy enough to do directly.
            byte[] vchMax = new byte[32]{
                0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFE,
                0xBA,0xAE,0xDC,0xE6,0xAF,0x48,0xA0,0x3B,
                0xBF,0xD2,0x5E,0x8C,0xD0,0x36,0x41,0x40
            };
            bool fIsZero = true;
            for (int i = 0; i < KeySize && fIsZero; i++)
                if (data[i] != 0)
                    fIsZero = false;
            if (fIsZero)
                return false;
            for (int i = 0; i < KeySize; i++)
            {
                if (data[i] < vchMax[i])
                    return true;
                if (data[i] > vchMax[i])
                    return false;
            }
            return true;
        }

        #endregion

    }
}

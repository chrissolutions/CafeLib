using System;
using System.Numerics;
using CafeLib.Core.Buffers;
using CafeLib.Core.Numerics;

namespace CafeLib.Cryptography.UnitTests.BsvSharp.Keys
{
    public class PrivateKey : IEquatable<PrivateKey>
    {
        private const int KeySize = UInt256.Length;
        private static ECKey _ecKey;

        /// <summary>
        /// PrivateKey data.
        /// </summary>
        private readonly UInt256 _keyData;

        /// <summary>
        /// HardenedBit.
        /// </summary>
        private const uint HardenedBit = 0x80000000;

        /// <summary>
        /// PrivateKey default constructor.
        /// </summary>
        public PrivateKey()
            : this(true)
        {
        }

        /// <summary>
        /// PrivateKey constructor.
        /// </summary>
        /// <param name="compressed"></param>
        public PrivateKey(bool compressed)
        {
            byte[] data;
            do
            {
                data = Randomizer.GetStrongRandBytes(KeySize);
            } while (!VerifyData(data));

            SetData(data, compressed);
        }

        /// <summary>
        /// PrivateKey constructor.
        /// </summary>
        /// <param name="keyData"></param>
        public PrivateKey(UInt256 keyData)
            : this(keyData, true)
        {
        }

        /// <summary>
        /// PrivateKey constructor.
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="compressed"></param>
        private PrivateKey(UInt256 keyData, bool compressed)
        {
            SetData(keyData, compressed);
        }

        /// <summary>
        /// PrivateKey constructor.
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="compressed"></param>
        public PrivateKey(string hex, bool compressed = true)
            : this(new UInt256(hex, compressed), compressed)
        {
        }

        /// <summary>
        /// Determines validity of the private key.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// True if the corresponding public key is compressed.
        /// </summary>
        public bool IsCompressed { get; private set; }

        public byte[] ToArray() => _keyData;

        public (PrivateKey keyChild, UInt256 ccChild) Derivate(UInt256 cc, uint nChild)
        {
            byte[] l = null;
            byte[] ll = new byte[32];
            byte[] lr = new byte[32];

            if ((nChild >> 31) == 0)
            {
                var pubKey = this.CreatePublicKey().ToArray();
                l = cc.Bip32Hash(nChild, pubKey[0], pubKey[1..]);
            }
            else
            {
                l = cc.Bip32Hash(nChild, 0, ToArray());
            }
            Array.Copy(l, ll, 32);
            Array.Copy(l, 32, lr, 0, 32);
            var ccChild = lr;

            BigInteger parse256LL = new BigInteger(1, ll);
            BigInteger kPar = new BigInteger(1, vch);
            BigInteger N = ECKey.CURVE.N;

            if (parse256LL.CompareTo(N) >= 0)
                throw new InvalidOperationException("You won a prize ! this should happen very rarely. Take a screenshot, and roll the dice again.");
            var key = parse256LL.Add(kPar).Mod(N);
            if (key == BigInteger.Zero)
                throw new InvalidOperationException("You won the big prize ! this would happen only 1 in 2^127. Take a screenshot, and roll the dice again.");

            var keyBytes = key.ToByteArrayUnsigned();
            if (keyBytes.Length < 32)
                keyBytes = new byte[32 - keyBytes.Length].Concat(keyBytes).ToArray();
            return new Key(keyBytes);
        }

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
            if (obj.GetType() != GetType()) return false;
            return Equals((PrivateKey) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private void SetData(ReadOnlyByteSpan data, bool compressed = true)
        {
            if (data.Length != UInt256.Length || !VerifyData(data))
            {
                IsValid = false;
            }
            else
            {
                var vch = new byte[KeySize];
                data.CopyTo(vch);
                IsCompressed = compressed;
                IsValid = true;
                _ecKey = new ECKey(vch, true);
            }
        }

        private bool VerifyData(ReadOnlyByteSpan data)
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

using System;
using CafeLib.Cryptography.BouncyCastle.Crypto;
using CafeLib.Cryptography.BouncyCastle.Crypto.Macs;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public abstract class HmacSha
    {
        public HMac HMac { get; }

        protected HmacSha(IDigest digest, byte[] key)
        {
            HMac = new HMac(digest);
            HMac.Init(new KeyParameter(key));
        }

        public virtual byte[] ComputeHash(byte[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var resBuf = new byte[HMac.GetMacSize()];
            HMac.BlockUpdate(value, 0, value.Length);
            HMac.DoFinal(resBuf, 0);
            return resBuf;
        }
    }
}

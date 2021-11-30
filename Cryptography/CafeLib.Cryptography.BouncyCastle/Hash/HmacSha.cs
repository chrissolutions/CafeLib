using System;
using CafeLib.Cryptography.BouncyCastle.Crypto;
using CafeLib.Cryptography.BouncyCastle.Crypto.Macs;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public abstract class HmacSha
    {
        private readonly HMac _hmac;

        protected HmacSha(IDigest digest, byte[] key)
        {
            _hmac = new HMac(digest);
            _hmac.Init(new KeyParameter(key));
        }

        public virtual byte[] ComputeHash(byte[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var resBuf = new byte[_hmac.GetMacSize()];
            _hmac.BlockUpdate(value, 0, value.Length);
            _hmac.DoFinal(resBuf, 0);

            return resBuf;
        }
    }
}

using System;
using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;
using CafeLib.Cryptography.BouncyCastle.Crypto.Macs;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public class HmacSha512
    {
        private readonly HMac _hmac;

        public HmacSha512(byte[] key)
        {
            _hmac = new HMac(new Sha512Digest());
            _hmac.Init(new KeyParameter(key));
        }

        public byte[] ComputeHash(byte[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            byte[] resBuf = new byte[_hmac.GetMacSize()];
            _hmac.BlockUpdate(value, 0, value.Length);
            _hmac.DoFinal(resBuf, 0);

            return resBuf;
        }
    }
}

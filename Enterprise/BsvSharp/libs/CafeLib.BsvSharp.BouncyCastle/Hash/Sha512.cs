using System;
using CafeLib.BsvSharp.BouncyCastle.Crypto;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Digests;

namespace CafeLib.BsvSharp.BouncyCastle.Hash
{
    public class Sha512
    {
        private readonly IDigest _digest;

        public Sha512()
        {
            _digest = new Sha512Digest();
        }

        public byte[] ComputeHash(byte[] data) => ComputeHash(data, data.Length);

        public byte[] ComputeHash(byte[] data, int count)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            _digest.BlockUpdate(data, 0, count);
            var rv = new byte[_digest.GetDigestSize()];
            _digest.DoFinal(rv, 0);
            return rv;
        }
    }
}

using System;
using CafeLib.Cryptography.BouncyCastle.Crypto;
using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public class Sha256
    {
        private readonly IDigest _digest;

        public Sha256()
        {
            _digest = new Sha256Digest();
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

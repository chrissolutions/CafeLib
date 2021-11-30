using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public class HmacSha512 : HmacSha
    {
        public HmacSha512(byte[] key)
            : base(new Sha512Digest(), key)
        {
        }
    }
}

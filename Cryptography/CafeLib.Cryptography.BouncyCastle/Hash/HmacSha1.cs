using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public class HmacSha1 : HmacSha
    {
        public HmacSha1(byte[] key)
            : base(new Sha1Digest(), key)
        {
        }
    }
}

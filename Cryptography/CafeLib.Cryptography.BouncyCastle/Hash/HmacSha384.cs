using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public class HmacSha384 : HmacSha
    {
        public HmacSha384(byte[] key)
            : base(new Sha384Digest(), key)
        {
        }
    }
}

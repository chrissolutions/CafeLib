﻿using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;

namespace CafeLib.Cryptography.BouncyCastle.Hash
{
    public class HmacSha256 : HmacSha
    {
        public HmacSha256(byte[] key)
            : base(new Sha256Digest(), key)
        {
        }
    }
}

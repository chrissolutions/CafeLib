using CafeLib.Core.Buffers;
using CafeLib.Core.Numerics;
using CafeLib.Cryptography.BouncyCastle.Asn1.X9;

// ReSharper disable InconsistentNaming

namespace CafeLib.Cryptography.UnitTests.BsvSharp.Keys
{
    public static class KeyExtensions
    {
        private static readonly X9ECParameters Secp256k1 = ECKey.CreateCurve();

        //private int SerializedPublicKeyLength => IsCompressed ? Secp256k1.SERIALIZED_COMPRESSED_PUBKEY_LENGTH : Secp256k1.SERIALIZED_UNCOMPRESSED_PUBKEY_LENGTH;

        public static PublicKey CreatePublicKey(this PrivateKey privateKey)
        {
            var ecKey = new ECKey(privateKey.ToArray(), true);
            var q = ecKey.GetPublicKeyParameters().Q;

            //Pub key (q) is composed into X and Y, the compressed form only include X, which can derive Y along with 02 or 03 prepent depending on whether Y in even or odd.
            var result = Secp256k1.Curve.CreatePoint(q.X.ToBigInteger(), q.Y.ToBigInteger(), privateKey.IsCompressed).GetEncoded();
            return new PublicKey(result);
        }

        /// <summary>
        /// The complement function is KzPubKey's RecoverCompact or KzPubKey.FromRecoverCompact.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static byte[] CreateCompactSignature(this PrivateKey privateKey, UInt256 hash)
        {
            var signer = new DeterministicECDSA();
            signer.SetPrivateKey(privateKey.ECKey.PrivateKey);
            var sig = ECDSASignature.FromDER(signer.SignHash(hash)).MakeCanonical();
            return sig.ToDER();
        }

        /// <summary>
        /// Create a signature from private key
        /// </summary>
        /// <param name="privateKey">private key</param>
        /// <param name="message">message to sign</param>
        /// <returns>signature bytes</returns>
        public static byte[] CreateSignature(this PrivateKey privateKey, ReadOnlyByteSpan message)
        {
            var signer = new DeterministicECDSA();
            signer.SetPrivateKey(privateKey.ECKey.PrivateKey);
            signer.Update(message);
            var results = signer.Sign();
            return results;
        }
    }
}

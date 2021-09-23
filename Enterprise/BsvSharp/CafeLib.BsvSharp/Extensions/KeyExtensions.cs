using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;
using CafeLib.Core.Extensions;
using CafeLib.Core.Numerics;
using CafeLib.Cryptography;
using CafeLib.Cryptography.BouncyCastle.Asn1.X9;

namespace CafeLib.BsvSharp.Extensions
{
    public static class KeyExtensions
    {
        // ReSharper disable once InconsistentNaming
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

        public static byte[] SignMessage(this PrivateKey key, string message)
            => SignMessage(key, message.Utf8ToBytes());

        public static byte[] SignMessage(this PrivateKey key, ReadOnlyByteSpan message)
            => key.CreateSignature(message);

        public static byte[] SignMessageCompact(this PrivateKey key, UInt256 hash)
            => key.CreateCompactSignature(hash);

        public static string SignMessageToBase64(this PrivateKey key, ReadOnlyByteSpan message)
        {
            var sigBytes = key.SignMessageCompact(GetMessageHash(message));
            return sigBytes == null ? null : Encoders.Base64.Encode(sigBytes);
        }

        public static string SignMessageToBase64(this PrivateKey key, string message)
            => SignMessageToBase64(key, message.Utf8ToBytes());

        public static bool VerifyMessage(this PublicKey key, string message, Signature signature)
        {
            var rkey = PublicKey.FromMessage(message, signature.ToString());
            return rkey != null && rkey == key;
        }

        public static bool VerifyMessage(this PublicKey key, string message, string signature)
        {
            var rkey = PublicKey.FromMessage(message, signature);
            return rkey != null && rkey == key;
        }

        public static bool VerifyMessage(this UInt160 keyId, UInt256 message, string signature)
        {
            var rkey = PublicKey.FromRecoverCompact(GetMessageHash(message.Span), Encoders.Base64.Decode(signature));
            return rkey != null && rkey.GetId() == keyId;
        }

        public static bool VerifyMessage(this UInt160 keyId, string message, string signature)
            => VerifyMessage(keyId, (UInt256)message.Utf8ToBytes(), signature);

        #region Helpers

        internal static UInt256 GetMessageHash(ReadOnlyByteSpan message)
        {
            const string bitcoinSignedMessageHeader = "Bitcoin Signed Message:\n";
            var bitcoinSignedMessageHeaderBytes = Encoders.Utf8.Decode(bitcoinSignedMessageHeader);
            var msgBytes = new [] {(byte)bitcoinSignedMessageHeaderBytes.Length}.Concat(bitcoinSignedMessageHeaderBytes, new VarInt((ulong)message.Length).ToArray(), message);
            return Hashes.Hash256(msgBytes);
        }

        #endregion
    }
}

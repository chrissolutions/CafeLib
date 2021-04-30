using System;
using System.Diagnostics;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Numerics;
using Secp256k1Net;

// ReSharper disable InconsistentNaming

namespace CafeLib.Bitcoin.Keys
{
    public static class KeyService
    {
        private static readonly Lazy<Secp256k1> LazySecpLibrary = new Lazy<Secp256k1>(() => {
            var ctx = new Secp256k1(sign: true, verify: false);
            ctx.Randomize(Randomizer.GetStrongRandBytes(32));
            return ctx;
        }, true);

        private static readonly Secp256k1 Library = LazySecpLibrary.Value;

        //private int SerializedPublicKeyLength => IsCompressed ? Secp256k1.SERIALIZED_COMPRESSED_PUBKEY_LENGTH : Secp256k1.SERIALIZED_UNCOMPRESSED_PUBKEY_LENGTH;

        public static PublicKey CreatePublicKey(this PrivateKey privateKey)
        {
            Trace.Assert(privateKey.IsValid);
            var pubKeySecp256k1 = new byte[Secp256k1.PUBKEY_LENGTH];
            var ok = Library.PublicKeyCreate(pubKeySecp256k1, privateKey.Bytes);
            Trace.Assert(ok);
            var pubKey = new PublicKey(privateKey.IsCompressed);
            LazySecpLibrary.Value.PublicKeySerialize((ByteSpan)pubKey, pubKeySecp256k1, privateKey.IsCompressed ? Flags.SECP256K1_EC_COMPRESSED : Flags.SECP256K1_EC_UNCOMPRESSED);
            Trace.Assert(pubKey.IsValid);
            return pubKey;
        }

        /// <summary>
        /// The complement function is KzPubKey's RecoverCompact or KzPubKey.FromRecoverCompact.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static byte[] CreateCompactSignature(this PrivateKey privateKey, UInt256 hash)
        {
            if (!privateKey.IsValid) return default;
            var (ok, sig) = Library.PrivateKeySignCompact(hash.Span, privateKey.Bytes, privateKey.IsCompressed);
            return ok ? sig : default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static byte[] CreateSignature(this PrivateKey privateKey, ReadOnlyByteSpan hash)
        {
            if (!privateKey.IsValid) return default;
            var (ok, sig) = Library.PrivateKeySign(hash, privateKey.Bytes);
            return ok ? sig : default;
        }
    }
}

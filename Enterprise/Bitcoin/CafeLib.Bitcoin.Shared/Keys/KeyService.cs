using System;
using System.Diagnostics;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;
using Secp256k1Net;

// ReSharper disable InconsistentNaming

namespace CafeLib.Bitcoin.Shared.Keys
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
            var ok = Library.PublicKeyCreate(pubKeySecp256k1, privateKey.ReadOnlySpan);
            Trace.Assert(ok);
            var pubKey = new PublicKey(privateKey.IsCompressed);
            LazySecpLibrary.Value.PublicKeySerialize(pubKey.Bytes, pubKeySecp256k1, privateKey.IsCompressed ? Flags.SECP256K1_EC_COMPRESSED : Flags.SECP256K1_EC_UNCOMPRESSED);
            Trace.Assert(pubKey.IsValid);
            return pubKey;
        }

        /// <summary>
        /// The complement function is KzPubKey's RecoverCompact or KzPubKey.FromRecoverCompact.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static (bool ok, byte[] sig) CreateCompactSignature(this PrivateKey privateKey, UInt256 hash)
        {
            if (!privateKey.IsValid) return (false, null);

            var (ok, sig) = Library.PrivateKeySignCompact(hash.Bytes, privateKey.ReadOnlySpan, privateKey.IsCompressed);

            return (ok, sig);
        }

        public static (bool ok, byte[] sig) CreateSignature(this PrivateKey privateKey, UInt256 hash)
        {
            if (!privateKey.IsValid) return (false, null);

            var (ok, sig) = Library.PrivateKeySign(hash.Bytes, privateKey.ReadOnlySpan);

            return (ok, sig);
        }

        public static bool TweakAdd(this PrivateKey privateKey, ByteSpan bytes, out PrivateKey childKey)
        {
            childKey = null;
            var dataChild = privateKey.ReadOnlySpan.ToUInt256();
            var result = Library.PrivKeyTweakAdd(dataChild.Bytes, bytes.Slice(0, 32));
            if (result)
            {
                childKey = new PrivateKey(dataChild);
            }

            return result;
        }
    }
}

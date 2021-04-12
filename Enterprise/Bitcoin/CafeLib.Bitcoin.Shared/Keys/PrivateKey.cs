#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Crypto;
using CafeLib.Bitcoin.Shared.Numerics;
using Secp256k1Net;

namespace CafeLib.Bitcoin.Shared.Keys
{
    /// <summary>
    /// 
    /// </summary>
    public class PrivateKey
    {
        private bool _fCompressed;
        private UInt256 _keyData;

        private Flags IsCompressedFlag => _fCompressed ? Flags.SECP256K1_EC_COMPRESSED : Flags.SECP256K1_EC_UNCOMPRESSED;

        public bool IsValid { get; private set; }

        private static int PublicKeyLength => Secp256k1.PUBKEY_LENGTH;
        int SerializedPublicKeyLength => _fCompressed ? Secp256k1.SERIALIZED_COMPRESSED_PUBKEY_LENGTH : Secp256k1.SERIALIZED_UNCOMPRESSED_PUBKEY_LENGTH;

        /// <summary>
        /// True if the corresponding public key is compressed.
        /// </summary>
        public bool IsCompressed => _fCompressed;

        public ReadOnlySpan<byte> ReadOnlySpan => _keyData.ReadOnlySpan;

        public BigInteger BigInteger => _keyData.ToBigInteger();

        private static bool Check(ReadOnlySpan<byte> vch)
        {
            using var library = new Secp256k1();
            return library.SecretKeyVerify(vch);
        }

        public PrivateKey(UInt256 keydata)
        {
            _keyData = keydata;
        }

        public PrivateKey(ReadOnlySpan<byte> span, UInt256 keydata, bool compressed = true)
        {
            _keyData = keydata;
            Set(span, compressed);
        }

        public PrivateKey(UInt256 v, UInt256 keydata, bool compressed = true)
        {
            _keyData = keydata;
            Set(v.ReadOnlySpan, compressed);
        }

        public PrivateKey(string hex, UInt256 keydata, bool compressed = true) : this(new UInt256(hex, firstByteFirst:true), keydata, compressed) { }

        //public static PrivateKey FromHex(string hex, bool compressed = true) => new PrivateKey(new UInt256(hex, firstByteFirst: true), compressed);
        //public static PrivateKey FromB58(string b58) => new Base58PrivateKey(b58).GetKey();
        //public static PrivateKey FromWIF(string wif) => new KzB58PrivKey(wif).GetKey();

        public void Set(ReadOnlyByteSpan data, bool compressed = true)
        {
            if (data.Length != _keyData.Length || !Check(data))
                IsValid = false;
            else {
                data.CopyTo(_keyData.Bytes);
                _fCompressed = compressed;
                IsValid = true;
            }
        }

        public void MakeNewKey(bool compressed)
        {
            do 
            {
                Randomizer.GetStrongRandBytes(_keyData.Bytes);
            } 
            while (!Check(_keyData.ReadOnlySpan));
            IsValid = true;
            _fCompressed = compressed;
        }

        public PublicKey GetPublicKey()
        {
            Trace.Assert(IsValid);
            var pubKeySecp256K1 = new byte[PublicKeyLength];
            var ok = Secp256K1.PublicKeyCreate(pubKeySecp256K1, _keyData.ReadOnlySpan);
            Trace.Assert(ok);
            var pubKey = new PublicKey(_fCompressed);
            Secp256K1.PublicKeySerialize(pubKey.Span, pubKeySecp256K1, IsCompressedFlag);
            Trace.Assert(pubKey.IsValid);
            return pubKey;
        }

        /// <summary>
        /// The complement function is KzPubKey's RecoverCompact or KzPubKey.FromRecoverCompact.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public (bool ok, byte[] sig) SignCompact(UInt256 hash)
        {
            if (!IsValid) return (false, null);

            var (ok, sig) = Secp256K1.PrivateKeySignCompact(hash.Bytes, _keyData.Bytes, IsCompressed);

            return (ok, sig);
        }

        public (bool ok, byte[] sig) Sign(UInt256 hash)
        {
            if (!IsValid) return (false, null);

            var (ok, sig) = Secp256K1.PrivateKeySign(hash.Bytes, _keyData.Bytes);

            return (ok, sig);
        }

        /// <summary>
        /// Verify thoroughly whether a private key and a public key match.
        /// This is done using a different mechanism than just regenerating it.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public bool VerifyPubKey(PublicKey publicKey)
        {
            if (publicKey.IsCompressed != _fCompressed)
                return false;

            var rnd = Randomizer.GetStrongRandBytes(8).ToArray();
            var str = "Bitcoin key verification\n";

            var hash = Hashes.Hash256(System.Text.Encoding.ASCII.GetBytes(str).Concat(rnd).ToArray());

            var (ok, sig) = Sign(hash);

            if (!ok) return false;

            return publicKey.Verify(hash, sig);
        }

        public (bool ok, PrivateKey keyChild, UInt256 ccChild) Derive(uint nChild, UInt256 cc)
        {
            if (!IsValid || !IsCompressed) goto fail;

            var vout = new byte[64];

            if (nChild < HardenedBit) {
                // Not hardened.
                var pubkey = GetPublicKey();
                Debug.Assert(pubkey.ReadOnlySpan.Length == 33);
                Hashes.Bip32Hash(cc, nChild, pubkey.ReadOnlySpan[0], pubkey.ReadOnlySpan.Slice(1), vout);
            } 
            else
            {
                // Hardened.
                Debug.Assert(_keyData.Bytes.Length == 32);
                Hashes.Bip32Hash(cc, nChild, 0, _keyData.Bytes, vout);
            }

            var sout = vout.AsSpan();
            var ccChild = new UInt256();
            sout.Slice(32, 32).CopyTo(ccChild.Bytes);

            var dataChild = new UInt256();
            _keyData.Bytes.CopyTo(dataChild.Bytes);

            var ok = Secp256K1.PrivKeyTweakAdd(dataChild.Bytes, sout.Slice(0, 32));
            if (!ok) goto fail;
            var keyChild = new PrivateKey(dataChild);
            return (ok, keyChild, ccChild);

        fail:
            return (false, null, UInt256.Zero);
        }

        public string ToHex() => _keyData.ToStringFirstByteFirst();
        //public KzB58PrivKey ToB58() => new KzB58PrivKey(this);
        //public override string ToString() => ToB58().ToString();

        public override int GetHashCode() => _keyData.GetHashCode();
        public bool Equals(PrivateKey o) => (object)o != null && _fCompressed.Equals(o._fCompressed) && _keyData.Equals(o._keyData);
        public override bool Equals(object obj) => obj is PrivateKey && this == (PrivateKey)obj;
        public static bool operator ==(PrivateKey x, PrivateKey y) => object.ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(PrivateKey x, PrivateKey y) => !(x == y);

        const uint HardenedBit = 0x80000000;

        private static readonly Lazy<Secp256k1> LazySecp256K1 = null;
        private Secp256k1 Secp256K1 => LazySecp256K1.Value;

        static PrivateKey()
        {
            LazySecp256K1 = new Lazy<Secp256k1>(() => {
                var ctx = new Secp256k1(sign: true, verify: false);
                ctx.Randomize(Randomizer.GetStrongRandBytes(32));
                return ctx;
            }, true);
        }

    }
}

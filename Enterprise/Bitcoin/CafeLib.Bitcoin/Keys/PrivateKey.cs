#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Numerics;
using Secp256k1Net;

namespace CafeLib.Bitcoin.Keys
{
    /// <summary>
    /// 
    /// </summary>
    public class PrivateKey
    {
        private readonly UInt256 _keyData;

        private const uint HardenedBit = 0x80000000;

        /// <summary>
        /// 
        /// </summary>
        private static readonly Lazy<Secp256k1> LazySecp256K1 = new Lazy<Secp256k1>(() =>
        {
            var ctx = new Secp256k1();
            ctx.Randomize(Randomizer.GetStrongRandBytes(32));
            return ctx;
        }, true);

        /// <summary>
        /// 
        /// </summary>
        private static Secp256k1 Secp256K1 => LazySecp256K1.Value;

        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// True if the corresponding public key is compressed.
        /// </summary>
        public bool IsCompressed { get; private set; }

        public ReadOnlyByteSpan Bytes => _keyData.Span;

        public BigInteger BigInteger => _keyData.ToBigInteger();

        private static bool Check(ReadOnlyByteSpan vch)
        {
            return Secp256K1.SecretKeyVerify(vch);
        }

        public PrivateKey()
        {
            do
            {
                Randomizer.GetStrongRandBytes(_keyData.Span);
            }
            while (!Check(_keyData));
            IsValid = true;
        }

        public PrivateKey(ReadOnlyByteSpan span, bool compressed = true)
        {
            Set(span, compressed);
        }

        public PrivateKey(ReadOnlyByteSpan span, UInt256 keyData, bool compressed = true)
        {
            _keyData = keyData;
            Set(span, compressed);
        }

        public PrivateKey(UInt256 keyData)
        {
            _keyData = keyData;
            IsCompressed = true;
            IsValid = true;
        }

        public PrivateKey(UInt256 v, bool compressed)
        {
            Set(v.Span, compressed);
        }

        public PrivateKey(UInt256 v, UInt256 keyData, bool compressed = true)
        {
            _keyData = keyData;
            Set(v.Span, compressed);
        }

        public PrivateKey(string hex, bool compressed = true)
            : this(new UInt256(hex, true), compressed)
        {
        }

        public PrivateKey(string hex, UInt256 keyData, bool compressed = true)
            : this(new UInt256(hex, true), keyData, compressed)
        {
        }

        public static PrivateKey FromHex(string hex, bool compressed = true) => new PrivateKey(new UInt256(hex, true), compressed);
        public static PrivateKey FromBase58(string base58) => new Base58PrivateKey(base58).GetKey();
        public static PrivateKey FromWif(string wif) => new Base58PrivateKey(wif).GetKey();
        public static PrivateKey FromRandom() => new PrivateKey();

        internal void Set(ReadOnlyByteSpan data, bool compressed = true)
        {
            if (data.Length != UInt256.Length || !Check(data))
                IsValid = false;
            else
            {
                data.CopyTo(_keyData.Span);
                IsCompressed = compressed;
                IsValid = true;
            }
        }

        /// <summary>
        /// Verify thoroughly whether a private key and a public key match.
        /// This is done using a different mechanism than just regenerating it.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public bool VerifyPubKey(PublicKey publicKey)
        {
            if (publicKey.IsCompressed != IsCompressed)
                return false;

            var rnd = Randomizer.GetStrongRandBytes(8).ToArray();
            const string str = "Bitcoin key verification\n";

            var hash = Hashes.Hash256(System.Text.Encoding.ASCII.GetBytes(str).Concat(rnd).ToArray());

            var sig = this.CreateSignature(hash);
            return sig != null && publicKey.Verify(hash, sig);
        }

        internal (bool ok, PrivateKey keyChild, UInt256 ccChild) Derive(uint nChild, UInt256 chainCode)
        {
            (bool, PrivateKey, UInt256) invalid = (false, null, UInt256.Zero);
            if (!IsValid || !IsCompressed) return invalid;

            var vout = new byte[64];

            if (nChild < HardenedBit) 
            {
                // Not hardened.
                var pubkey = this.CreatePublicKey();
                Debug.Assert(pubkey.Data.Length == PublicKey.CompressedLength);
                Hashes.Bip32Hash(chainCode, nChild, pubkey.Data[0], pubkey.Data[1..], vout);
            } 
            else
            {
                // Hardened.
                Debug.Assert(_keyData.Span.Length == UInt256.Length);
                Hashes.Bip32Hash(chainCode, nChild, 0, _keyData.Span, vout);
            }

            var output = vout.AsSpan();
            var ccChild = new UInt256();
            output.Slice(UInt256.Length, UInt256.Length).CopyTo(ccChild.Span);

            var dataChild = new UInt256(_keyData);
            return Secp256K1.PrivKeyTweakAdd(dataChild.Span, output[..UInt256.Length])
                ? (true, new PrivateKey(dataChild), ccChild)
                : invalid;
        }

        public string ToHex() => _keyData.ToStringFirstByteFirst();
        public Base58PrivateKey ToBase58() => new Base58PrivateKey(this);
        public override string ToString() => ToBase58().ToString();

        public override int GetHashCode() => _keyData.GetHashCode();
        public bool Equals(PrivateKey o) => !(o is null) && IsCompressed.Equals(o.IsCompressed) && _keyData.Equals(o._keyData);
        public override bool Equals(object obj) => obj is PrivateKey key && this == key;

        public static bool operator ==(PrivateKey x, PrivateKey y) => x?.Equals(y) ?? y is null;
        public static bool operator !=(PrivateKey x, PrivateKey y) => !(x == y);
    }
}

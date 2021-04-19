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

namespace CafeLib.Bitcoin.Keys
{
    /// <summary>
    /// 
    /// </summary>
    public class PrivateKey
    {
        private readonly UInt256 _keyData;

        private const uint HardenedBit = 0x80000000;

        public bool IsValid { get; private set; }

        /// <summary>
        /// True if the corresponding public key is compressed.
        /// </summary>
        public bool IsCompressed { get; private set; }

        public ReadOnlyByteSpan Bytes => _keyData.Span;

        public BigInteger BigInteger => _keyData.ToBigInteger();

        private static bool Check(ReadOnlyByteSpan vch)
        {
            return KeyService.SecretKeyVerify(vch);
        }

        public PrivateKey()
        {
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
            : this(new UInt256(hex, true), keyData, compressed) { }

        public static PrivateKey FromHex(string hex, bool compressed = true) => new PrivateKey(new UInt256(hex, true), compressed);
        public static PrivateKey FromBase58(string base58) => new Base58PrivateKey(base58).GetKey();
        public static PrivateKey FromWIF(string wif) => new Base58PrivateKey(wif).GetKey();

        public void Set(ReadOnlyByteSpan data, bool compressed = true)
        {
            if (data.Length != UInt256.Length || !Check(data))
                IsValid = false;
            else {
                data.CopyTo(_keyData.Span);
                IsCompressed = compressed;
                IsValid = true;
            }
        }

        public void MakeNewKey(bool compressed)
        {
            do 
            {
                Randomizer.GetStrongRandBytes(_keyData.Span);
            } 
            while (!Check(_keyData.Span));
            IsValid = true;
            IsCompressed = compressed;
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
            return (sig != null) && publicKey.Verify(hash, sig);
        }

        public (bool ok, PrivateKey keyChild, UInt256 ccChild) Derive(uint nChild, UInt256 cc)
        {
            if (!IsValid || !IsCompressed) goto fail;

            var vout = new byte[64];

            if (nChild < HardenedBit) {
                // Not hardened.
                var pubkey = this.CreatePublicKey();
                Debug.Assert(pubkey.ReadOnlySpan.Length == 33);
                Hashes.Bip32Hash(cc, nChild, pubkey.ReadOnlySpan[0], pubkey.ReadOnlySpan.Slice(1), vout);
            } 
            else
            {
                // Hardened.
                Debug.Assert(_keyData.Span.Length == 32);
                Hashes.Bip32Hash(cc, nChild, 0, _keyData.Span, vout);
            }

            var sout = vout.AsSpan();
            var ccChild = new UInt256();
            sout.Slice(32, 32).CopyTo(ccChild.Span);

            var ok = this.TweakAdd( sout.Slice(0, 32), out var keyChild);
            if (!ok) goto fail;
            return (true, keyChild, ccChild);

        fail:
            return (false, null, UInt256.Zero);
        }

        public string ToHex() => _keyData.ToStringFirstByteFirst();
        public Base58ExtPrivateKey ToBase58() => new Base58ExtPrivateKey();
        public override string ToString() => ToBase58().ToString();

        public override int GetHashCode() => _keyData.GetHashCode();
        public bool Equals(PrivateKey o) => (object)o != null && IsCompressed.Equals(o.IsCompressed) && _keyData.Equals(o._keyData);
        public override bool Equals(object obj) => obj is PrivateKey key && this == key;
        public static bool operator ==(PrivateKey x, PrivateKey y) => (object)x != null && (ReferenceEquals(x, y) || x.Equals(y));
        public static bool operator !=(PrivateKey x, PrivateKey y) => !(x == y);
    }
}

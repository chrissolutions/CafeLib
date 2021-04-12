﻿#region Copyright
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
        private readonly UInt256 _keyData;

        private const uint HardenedBit = 0x80000000;

        public bool IsValid { get; private set; }

        /// <summary>
        /// True if the corresponding public key is compressed.
        /// </summary>
        public bool IsCompressed { get; private set; }

        public ReadOnlyByteSpan ReadOnlySpan => _keyData.ReadOnlySpan;

        public BigInteger BigInteger => _keyData.ToBigInteger();

        private static bool Check(ReadOnlySpan<byte> vch)
        {
            using var library = new Secp256k1();
            return library.SecretKeyVerify(vch);
        }

        public PrivateKey()
        {
        }

        public PrivateKey(ReadOnlySpan<byte> span, bool compressed = true)
        {
            Set(span, compressed);
        }

        public PrivateKey(ReadOnlySpan<byte> span, UInt256 keyData, bool compressed = true)
        {
            _keyData = keyData;
            Set(span, compressed);
        }

        public PrivateKey(UInt256 keyData)
        {
            _keyData = keyData;
        }

        public PrivateKey(UInt256 v, bool compressed = true)
        {
            Set(v.ReadOnlySpan, compressed);
        }

        public PrivateKey(UInt256 v, UInt256 keyData, bool compressed = true)
        {
            _keyData = keyData;
            Set(v.ReadOnlySpan, compressed);
        }

        public PrivateKey(string hex, bool compressed = true)
            : this(new UInt256(hex, true), compressed)
        {
        }

        public PrivateKey(string hex, UInt256 keyData, bool compressed = true)
            : this(new UInt256(hex, true), keyData, compressed) { }

        public static PrivateKey FromHex(string hex, bool compressed = true) => new PrivateKey(new UInt256(hex, true), compressed);
        //public static KzPrivKey FromB58(string b58) => new KzB58PrivKey(b58).GetKey();
        //public static KzPrivKey FromWIF(string wif) => new KzB58PrivKey(wif).GetKey();

        public void Set(ReadOnlyByteSpan data, bool compressed = true)
        {
            if (data.Length != _keyData.Length || !Check(data))
                IsValid = false;
            else {
                data.CopyTo(_keyData.Bytes);
                IsCompressed = compressed;
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
            var str = "Bitcoin key verification\n";

            var hash = Hashes.Hash256(System.Text.Encoding.ASCII.GetBytes(str).Concat(rnd).ToArray());

            var (ok, sig) = this.CreateSignature(hash);

            if (!ok) return false;

            return publicKey.Verify(hash, sig);
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
                Debug.Assert(_keyData.Bytes.Length == 32);
                Hashes.Bip32Hash(cc, nChild, 0, _keyData.Bytes, vout);
            }

            var sout = vout.AsSpan();
            var ccChild = new UInt256();
            sout.Slice(32, 32).CopyTo(ccChild.Bytes);

            var ok = this.TweakAdd( sout.Slice(0, 32), out var keyChild);
            if (!ok) goto fail;
            return (true, keyChild, ccChild);

        fail:
            return (false, null, UInt256.Zero);
        }

        public string ToHex() => _keyData.ToStringFirstByteFirst();
        //public KzB58PrivKey ToB58() => new KzB58PrivKey(this);
        //public override string ToString() => ToB58().ToString();

        public override int GetHashCode() => _keyData.GetHashCode();
        public bool Equals(PrivateKey o) => (object)o != null && IsCompressed.Equals(o.IsCompressed) && _keyData.Equals(o._keyData);
        public override bool Equals(object obj) => obj is PrivateKey key && this == key;
        public static bool operator ==(PrivateKey x, PrivateKey y) => object.ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(PrivateKey x, PrivateKey y) => !(x == y);
    }
}

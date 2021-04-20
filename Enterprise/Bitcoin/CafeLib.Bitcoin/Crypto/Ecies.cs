#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Encoding;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Numerics;
using Secp256k1Net;

namespace CafeLib.Bitcoin.Crypto
{
    /// <summary>
    /// Two parties set matching pairs of private and public keys.
    /// e.g. Alice sets her private key and Bob's public key.
    /// Bob sets his own private key and Alice's public key.
    /// The key pairs allow the derivation of two shared keys: kE and kM.
    /// kE is used as the encryption / decryption key.
    /// kM is used to sign the encrypted data to verify that what is received is what was sent.
    /// By default, the sender's pub key can be sent as part of the message.
    /// Set NoKey = true to omit sender's public key from the message sent. 
    /// Set ShortTag = true to reduce the content verification signature from 32 to 4 bytes.
    /// </summary>
    public class Ecies
    {
        private PrivateKey _privateKey;
        private PublicKey _publicKey;
        private UInt256 _kE;
        private UInt256 _kM;

        public PrivateKey PrivateKey
        {
            get => _privateKey; 
            set 
            { 
                _privateKey = value; 
                UpdatekEkM();
            }
        }

        public PublicKey PublicKey
        {
            get => _publicKey;
            set
            {
                _publicKey = value;
                UpdatekEkM();
            }
        }

        public bool ShortTag { get; set; }
        public bool NoKey { get; set; }

        /// <summary>
        /// Two parties set matching pairs of priv and pub keys.
        /// e.g. Alice sets her priv key and Bob's pub key. Bob sets his own priv key and Alice's pub key.
        /// And the values of _kE and _kM will be equal.
        /// _kE is used as the encryption / decryption key.
        /// _kM is used to sign the encrypted data to verify that what is received is what was sent.
        /// </summary>
        private void UpdatekEkM()
        {
            if (_privateKey != null && _publicKey != null && _publicKey.IsValid)
            {
                using var secp = new Secp256k1();
                var k = _publicKey.Clone();
                // Multiply the public key as an elliptic curve point by the private key a big number: 
                var bn = _privateKey.BigInteger;
                var pkbs = new byte[64];
                if (!secp.PublicKeyParse(pkbs.AsSpan(), _publicKey.ReadOnlySpan)) goto fail;
                if (!secp.PubKeyTweakMul(pkbs.AsSpan(), _privateKey.Bytes)) goto fail;
                // Hash the X coordinate of the resulting elliptic curve point.
                var x = pkbs.Slice(0, 32);
                x.Reverse();
                var h = Hashes.Sha512(x).Span;
                _kE = new UInt256(h.Slice(0, 32));
                _kM = new UInt256(h.Slice(32, 32));
                fail:
                ;
            }
        }

        public byte[] Encrypt(string message) => Encrypt(message.Utf8ToBytes());

        public string DecryptToUtf8(ReadOnlyByteSpan data) => Encoders.Utf8.Encode(Decrypt(data));

        public byte[] Encrypt(ReadOnlyByteSpan data)
        {
            var iv = Encryption.InitializationVector(_privateKey.Bytes, data);

            var c = Encryption.AesEncrypt(data, _kE.Span, iv);
            //var c = AESCBC_Encrypt(data.ToArray(), _kE.ToBytes(), iv);
            var d = Hashes.HmacSha256(_kM.Span, c).Span;
            if (ShortTag) d = d.Slice(0, 4);

            var key = NoKey ? ReadOnlyByteSpan.Empty : _privateKey.CreatePublicKey().ReadOnlySpan;
            var len = key.Length + c.Length + d.Length;
            var bytes = new byte[len];
            var result = bytes.AsSpan();
            key.CopyTo(result.Slice(0));
            c.CopyTo(result.Slice(key.Length));
            d.CopyTo(result.Slice(key.Length + c.Length));
            return bytes;
        }

        public byte[] Decrypt(ReadOnlyByteSpan data)
        {
            // data is either:
            // NoKey == true
            // 16 byte IV, <encrypted data>, 4 (ShortTag) or 32 byte d (signature)
            // NoKey == false
            // 33 byte pub key, 16 byte IV, <encrypted data>, 4 (ShortTag) or 32 byte d (signature)
            var key = NoKey ? ReadOnlyByteSpan.Empty : data.Slice(0, 33);
            var cd = NoKey ? data : data.Slice(33);
            var dLen = ShortTag ? 4 : 32;
            var d = cd.Slice(cd.Length - dLen);
            var c = cd.Slice(0, cd.Length - dLen);

            if (!NoKey)
            {
                // The data includes the sender's public key. Use it.
                PublicKey = new PublicKey(key);
            }

            var d1 = Hashes.HmacSha256(_kM.Span, c).Span;
            if (ShortTag)
                d1 = d1.Slice(0, 4);

            var ok = d.ToHex() == d1.ToHex();
            if (!ok)
            {
                // Signature fails.
                return null;
            }

            var r = Encryption.AesDecrypt(c, _kE);

            return r;
        }
    }
}

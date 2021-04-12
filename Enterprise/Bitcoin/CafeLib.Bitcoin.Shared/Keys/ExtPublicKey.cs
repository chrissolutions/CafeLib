﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;

namespace CafeLib.Bitcoin.Shared.Keys
{

    public class ExtPublicKey : ExtKey
    {
        public PublicKey PublicKey { get; private set; }

        public ExtPublicKey() { }

        public ExtPublicKey(ReadOnlyByteSpan code)
        {
            Decode(code);
        }

        //public static ExtPublicKey FromPriv(ExtPrivateKey priv)
        //{
        //    var key = new ExtPublicKey {
        //        _depth = priv.Depth,
        //        _fingerprint = priv.Fingerprint,
        //        _child = priv.IndexWithHardened,
        //        _chaincode = priv.Chaincode,
        //        PublicKey = priv.PrivKey.GetPubKey()
        //    };

        //    return key;
        //}

        /// <summary>
        /// Computes the public key specified by a key path.
        /// At each derivation, there's a small chance the index specified will fail.
        /// If any generation fails, null is returned.
        /// </summary>
        /// <param name="kp"></param>
        /// <returns>null on derivation failure. Otherwise the derived private key.</returns>
        public ExtPublicKey Derive(KeyPath kp) => DeriveBase(kp) as ExtPublicKey;

        public ExtPublicKey Derive(int index) => DeriveBase(index, false) as ExtPublicKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hardened"></param>
        /// <returns></returns>
        public override ExtKey DeriveBase(int index, bool hardened)
        {
            var cek = new ExtPublicKey 
            {
                Depth = (byte)(Depth + 1),
                Child = (uint)index | (hardened ? HardenedBit : 0),
                Fingerprint = BitConverter.ToInt32(PublicKey.GetId().Bytes.Slice(0, 4))
            };

            bool ok;
            (ok, cek.PublicKey, cek.ChainCode) = PublicKey.Derive(cek.Child, ChainCode);
            return ok ? cek : null;
        }

        public byte[] GetBytes() {
            var bytes = new byte[Bip32KeySize];
            Encode(bytes);
            return bytes;
        }

        public override void Encode(ByteSpan code)
        {
            code[0] = Depth;

            Fingerprint.AsSpan().CopyTo(code.Slice(1, 4));
            code[5] = (byte)((Child >> 24) & 0xFF);
            code[6] = (byte)((Child >> 16) & 0xFF);
            code[7] = (byte)((Child >> 8) & 0xFF);
            code[8] = (byte)((Child >> 0) & 0xFF);
            ChainCode.Bytes.CopyTo(code.Slice(9, 32));
            var key = PublicKey.ReadOnlySpan;
            Debug.Assert(key.Length == 33);
            key.CopyTo(code.Slice(41, 33));
        }

        public void Decode(ReadOnlyByteSpan code)
        {
            Depth = code[0];
            Fingerprint = BitConverter.ToInt32(code.Slice(1, 4));
            Child = (uint)code[5] << 24 | (uint)code[6] << 16 | (uint)code[7] << 8 | (uint)(code[8]);
            code.Slice(9, 32).CopyTo(ChainCode.Bytes);
            PublicKey = new PublicKey();
            PublicKey.Set(code.Slice(41, 33));
        }

        //public Base58ExtPublicKey ToBase58() => new Base58ExtPublicKey(this);
        //public override string ToString() => ToBase58().ToString();

        public override int GetHashCode() => base.GetHashCode() ^ PublicKey.GetHashCode();
        public bool Equals(ExtPublicKey o) => (object)o != null && base.Equals(o) && PublicKey == o.PublicKey;
        public override bool Equals(object obj) => obj is ExtPublicKey key && this == key;
        public static bool operator ==(ExtPublicKey x, ExtPublicKey y) => object.ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(ExtPublicKey x, ExtPublicKey y) => !(x == y);
    }
}
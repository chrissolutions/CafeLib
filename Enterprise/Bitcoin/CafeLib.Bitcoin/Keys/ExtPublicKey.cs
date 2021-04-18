#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Extensions;

namespace CafeLib.Bitcoin.Keys
{

    public class ExtPublicKey : ExtKey
    {
        public PublicKey PublicKey { get; private set; }

        public ExtPublicKey()
        {
        }

        public ExtPublicKey(ReadOnlyByteSpan code)
        {
            Decode(code);
        }

        public static ExtPublicKey FromPrivate(ExtPrivateKey priv)
        {
            var key = new ExtPublicKey
            {
                Depth = priv.Depth,
                Fingerprint = priv.Fingerprint,
                Child = priv.IndexWithHardened,
                ChainCode = priv.ChainCode,
                PublicKey = priv.PrivateKey.CreatePublicKey()
            };

            return key;
        }

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
                Fingerprint = BitConverter.ToInt32(PublicKey.GetId().Span.Slice(0, 4))
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
            ChainCode.Span.CopyTo(code.Slice(9, 32));
            var key = PublicKey.ReadOnlySpan;
            Debug.Assert(key.Length == 33);
            key.CopyTo(code.Slice(41, 33));
        }

        public void Decode(ReadOnlyByteSpan code)
        {
            Depth = code[0];
            Fingerprint = BitConverter.ToInt32(code.Slice(1, 4));
            Child = (uint)code[5] << 24 | (uint)code[6] << 16 | (uint)code[7] << 8 | (uint)(code[8]);
            code.Slice(9, 32).CopyTo(ChainCode.Span);
            PublicKey = new PublicKey();
            PublicKey.Set(code.Slice(41, 33));
        }

        //public Base58ExtPublicKey ToBase58() => new Base58ExtPublicKey(this);
        //public override string ToString() => ToBase58().ToString();

        public override int GetHashCode() => base.GetHashCode() ^ PublicKey.GetHashCode();
        public bool Equals(ExtPublicKey o) => (object)o != null && base.Equals(o) && PublicKey == o.PublicKey;
        public override bool Equals(object obj) => obj is ExtPublicKey key && this == key;
        public static bool operator ==(ExtPublicKey x, ExtPublicKey y) => x != null && (ReferenceEquals(x, y) || x.Equals(y));
        public static bool operator !=(ExtPublicKey x, ExtPublicKey y) => !(x == y);
    }
}

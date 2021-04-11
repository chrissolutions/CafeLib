#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Numerics;

namespace CafeLib.Bitcoin.Shared.Keys
{
    public abstract class ExtendedKey
    {
        public const uint HardenedBit = 0x80000000;
        public const int Bip32KeySize = 74;

        public byte Depth { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        protected uint Child { get; set; }

        /// <summary>
        /// First four bytes of the corresponding public key's HASH160 which is also called its key ID.
        /// </summary>
        public int Fingerprint { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Hardened => Child >= HardenedBit;

        /// <summary>
        /// 
        /// </summary>
        public UInt256 ChainCode { get; protected set; }

        /// <summary>
        /// Always excludes the HardenedBit.
        /// </summary>
        public int Index => (int)(Child & ~HardenedBit);

        /// <summary>
        /// 
        /// </summary>
        public uint IndexWithHardened => Child;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public abstract void Encode(ByteSpan code);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hardened"></param>
        /// <returns></returns>
        public abstract ExtendedKey DeriveBase(int index, bool hardened);

        /// <summary>
        /// Computes the key specified by a key path.
        /// At each derivation, there's a small chance the index specified will fail.
        /// If any generation fails, null is returned.
        /// </summary>
        /// <param name="kp"></param>
        /// <returns>null on derivation failure. Otherwise the derived private key.</returns>
        public ExtendedKey DeriveBase(KeyPath kp)
        {
            var k = this;
            foreach (var i in kp.Indices)
            {
                k = k.DeriveBase((int)(i & ~HardenedBit), (i & HardenedBit) != 0);
                if (k == null) break;
            }
            return k;
        }

        public override int GetHashCode() => Depth.GetHashCode() ^ Fingerprint.GetHashCode() ^ Child.GetHashCode() ^ ChainCode.GetHashCode();
        public bool Equals(ExtendedKey o) => (object)o != null && Depth == o.Depth && Fingerprint == o.Fingerprint && Child == o.Child && ChainCode == o.ChainCode;
        public override bool Equals(object obj) => obj is ExtendedKey key && Equals(key);
        public static bool operator ==(ExtendedKey x, ExtendedKey y) => object.ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(ExtendedKey x, ExtendedKey y) => !(x == y);
    }
}

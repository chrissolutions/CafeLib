#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using CafeLib.Bitcoin.Numerics;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.Bitcoin.Chain {

    public struct UtxoKey
    {

        public UInt256 Txid;
        public Int32 N;

        public override string ToString() {
            return $"{Txid} {N}";
        }

        public void Write(BinaryWriter s) {
            s.Write(N);
            s.Write(Txid.Span);
        }

        public void Read(BinaryReader s) {
            N = s.ReadInt32();
            Txid.Read(s);
        }

        public override bool Equals(object obj) => obj is UtxoKey key && this == key;
        public override int GetHashCode() => Txid.GetHashCode() ^ N;

        public bool Equals(UtxoKey o) => Txid == o.Txid && N == o.N;
        public static bool operator ==(UtxoKey x, UtxoKey y) => x.Equals(y);
        public static bool operator !=(UtxoKey x, UtxoKey y) => !(x == y);
    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using CafeLib.Bitcoin.Shared.Numerics;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.Bitcoin.Shared.Chain {

    public struct UtxoKey
    {

        public UInt256 TxId;
        public Int32 N;

        public override string ToString() {
            return $"{TxId} {N}";
        }

        public void Write(BinaryWriter s) {
            s.Write(N);
            s.Write(TxId.Bytes);
        }

        public void Read(BinaryReader s) {
            N = s.ReadInt32();
            TxId.Read(s);
        }

        public override bool Equals(object obj) => obj is UtxoKey key && this == key;
        public override int GetHashCode() => TxId.GetHashCode() ^ N;

        public bool Equals(UtxoKey o) => TxId == o.TxId && N == o.N;
        public static bool operator ==(UtxoKey x, UtxoKey y) => x.Equals(y);
        public static bool operator !=(UtxoKey x, UtxoKey y) => !(x == y);
    }
}

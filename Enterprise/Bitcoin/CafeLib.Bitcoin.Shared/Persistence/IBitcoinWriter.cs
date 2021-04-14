#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Numerics;

namespace CafeLib.Bitcoin.Shared.Persistence
{
    public interface IBitcoinWriter
    {
        public IBitcoinWriter Add(ReadOnlyByteSpan data);
        public IBitcoinWriter Add(ReadOnlyByteSequence data);

        public IBitcoinWriter Add(UInt64 v);
        public IBitcoinWriter Add(Int64 v);
        public IBitcoinWriter Add(UInt32 v);
        public IBitcoinWriter Add(Int32 v);
        public IBitcoinWriter Add(byte v);
        public IBitcoinWriter Add(UInt160 v);
        public IBitcoinWriter Add(UInt256 v);
        public IBitcoinWriter Add(UInt512 v);
    }
}

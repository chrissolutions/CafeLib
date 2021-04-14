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
        IBitcoinWriter Add(ReadOnlyByteSpan data);
        IBitcoinWriter Add(ReadOnlyByteSequence data);

        IBitcoinWriter Add(byte v);
        IBitcoinWriter Add(int v);
        IBitcoinWriter Add(uint v);
        IBitcoinWriter Add(long v);
        IBitcoinWriter Add(ulong v);
        IBitcoinWriter Add(UInt160 v);
        IBitcoinWriter Add(UInt256 v);
        IBitcoinWriter Add(UInt512 v);
    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using CafeLib.Bitcoin.Shared.Numerics;

namespace CafeLib.Bitcoin.Shared.Persistence
{
    public interface IKzWriter
    {
        public IKzWriter Add(ReadOnlySpan<byte> data);
        public IKzWriter Add(ReadOnlySequence<byte> data);

        public IKzWriter Add(UInt64 v);
        public IKzWriter Add(Int64 v);
        public IKzWriter Add(UInt32 v);
        public IKzWriter Add(Int32 v);
        public IKzWriter Add(byte v);
        public IKzWriter Add(UInt160 v);
        public IKzWriter Add(UInt256 v);
        public IKzWriter Add(UInt512 v);
    }
}

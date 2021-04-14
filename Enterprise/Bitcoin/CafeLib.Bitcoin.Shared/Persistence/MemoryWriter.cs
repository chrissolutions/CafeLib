#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Bitcoin.Shared.Persistence
{
    public class MemoryWriter : IBitcoinWriter
    {
        private readonly Memory<byte> _memory;
        public int Length => _memory.Length;

        public MemoryWriter()
        {
            _memory = new Memory<byte>();
        }

        public MemoryWriter(Memory<byte> memory)
        {
            _memory = memory;
        }

        public IBitcoinWriter Add(ReadOnlyByteSpan data)
        {
            data.CopyTo(_memory.Span); 
            return this;
        }

        public IBitcoinWriter Add(ReadOnlyByteSequence data)
        {
            data.CopyTo(_memory.Span[Length..]);
            return this;
        }

        public IBitcoinWriter Add(byte v)
        {
            _memory.Span[Length] = v; 
            return this;
        }

        public IBitcoinWriter Add(int v)
        {
            v.AsSpan().CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(uint v)
        {
            v.AsSpan().CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(long v)
        {
            v.AsSpan().CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(ulong v)
        {
            v.AsSpan().CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt160 v)
        {
            v.Bytes.CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt256 v)
        {
            v.Bytes.CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt512 v)
        {
            v.Bytes.CopyTo(_memory.Span[Length..]);
            return this;
        }
    }
}

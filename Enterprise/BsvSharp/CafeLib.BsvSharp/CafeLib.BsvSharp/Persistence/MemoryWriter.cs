#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Persistence
{
    public class MemoryWriter : IBitcoinWriter
    {
        private readonly ByteMemory _memory;
        public int Length => _memory.Length;

        public MemoryWriter()
        {
            _memory = new ByteMemory();
        }

        public MemoryWriter(ByteMemory memory)
        {
            _memory = memory;
        }

        public IBitcoinWriter Add(byte[] data)
        {
            data.CopyTo(_memory.Data);
            return this;
        }

        public IBitcoinWriter Add(byte v)
        {
            _memory[Length] = v; 
            return this;
        }

        public IBitcoinWriter Add(int v)
        {
            v.AsSpan().CopyTo(_memory[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(uint v)
        {
            v.AsSpan().CopyTo(_memory[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(long v)
        {
            v.AsSpan().CopyTo(_memory[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(ulong v)
        {
            v.AsSpan().CopyTo(_memory.Data.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt160 v)
        {
            v.Span.CopyTo(_memory[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt256 v)
        {
            v.Span.CopyTo(_memory[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt512 v)
        {
            v.Span.CopyTo(_memory[Length..]);
            return this;
        }
    }
}

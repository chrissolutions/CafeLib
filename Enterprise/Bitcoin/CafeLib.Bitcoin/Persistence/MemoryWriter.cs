﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Persistence
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

        public IBitcoinWriter Add(byte[] data)
        {
            data.CopyTo(_memory.Span);
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
            v.Span.CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt256 v)
        {
            v.Span.CopyTo(_memory.Span[Length..]); 
            return this;
        }

        public IBitcoinWriter Add(UInt512 v)
        {
            v.Span.CopyTo(_memory.Span[Length..]);
            return this;
        }
    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;

namespace CafeLib.Bitcoin.Shared.Persistence
{
    public class MemoryWriter : IBitcoinWriter
    {
        public int Length;
        public Memory<byte> Memory;

        public MemoryWriter(Memory<byte> memory)
        {
            Memory = memory;
        }

        public IBitcoinWriter Add(ReadOnlyByteSpan data)
        {
            data.CopyTo(Memory.Span[Length..]); 
            Length += data.Length; 
            return this;
        }

        public IBitcoinWriter Add(ReadOnlyByteSequence data)
        {
            data.CopyTo(Memory.Span[Length..]); 
            Length += (int)data.Length;
            return this;
        }

        public IBitcoinWriter Add(byte v)
        {
            Memory.Span[Length] = v; 
            Length += 1; 
            return this;
        }

        public IBitcoinWriter Add(int v)
        {
            v.AsSpan().CopyTo(Memory.Span[Length..]); 
            Length += sizeof(int); 
            return this;
        }

        public IBitcoinWriter Add(uint v)
        {
            v.AsSpan().CopyTo(Memory.Span[Length..]); 
            Length += sizeof(uint); 
            return this;
        }

        public IBitcoinWriter Add(long v)
        {
            v.AsSpan().CopyTo(Memory.Span[Length..]); 
            Length += sizeof(long); 
            return this;
        }

        public IBitcoinWriter Add(ulong v)
        {
            v.AsSpan().CopyTo(Memory.Span[Length..]); 
            Length += sizeof(ulong); 
            return this;
        }

        public IBitcoinWriter Add(UInt160 v)
        {
            v.Bytes.CopyTo(Memory.Span[Length..]); 
            Length += UInt160.Length; 
            return this;
        }

        public IBitcoinWriter Add(UInt256 v)
        {
            v.Bytes.CopyTo(Memory.Span[Length..]); 
            Length += 32; 
            return this;
        } 

        public IBitcoinWriter Add(UInt512 v) { v.Bytes.CopyTo(Memory.Span.Slice(Length)); Length += 64; return this; } 
    }
}

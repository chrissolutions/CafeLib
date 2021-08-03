#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Persistence
{
    public class LengthWriter : IDataWriter
    {
        public long Length { get; private set; }

        public IDataWriter Write(ReadOnlyByteSpan data)
        {
            Length += data.Length;
            return this;
        }

        public IDataWriter Add(ReadOnlyByteSequence data)
        {
            Length += data.Length;
            return this;
        }

        public IDataWriter Write(byte[] data)
        {
            Length += data.Length;
            return this;
        }

        public IDataWriter Write(byte v)
        {
            Length += sizeof(byte);
            return this;
        }

        public IDataWriter Write(long data)
        {
            Length += sizeof(long);
            return this;
        }

        public IDataWriter Write(ulong data)
        {
            Length += sizeof(ulong); 
            return this;
        }

        public IDataWriter Write(string data)
        {
            Length += data.Length * sizeof(char);
            return this;
        }

        public IDataWriter Write(int data)
        {
            Length += sizeof(int);
            return this;
        }

        public IDataWriter Write(uint data)
        {
            Length += sizeof(uint); 
            return this;
        }
        
        public IDataWriter Write(UInt160 v)
        {
            Length += UInt160.Length; 
            return this;
        }

        public IDataWriter Write(UInt256 v)
        {
            Length += UInt256.Length; 
            return this;
        }

        public IDataWriter Write(UInt512 v)
        {
            Length += UInt256.Length; 
            return this;
        } 
    }
}

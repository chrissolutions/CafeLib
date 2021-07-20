#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Persistence
{
    public class LengthWriter : IBitcoinWriter
    {
        public long Length { get; private set; }

        public IBitcoinWriter Add(byte[] data)
        {
            Length += data.Length;
            return this;
        }

        public IBitcoinWriter Add(ReadOnlyByteSpan data)
        {
            Length += data.Length; 
            return this;
        }

        public IBitcoinWriter Add(ReadOnlyByteSequence data)
        {
            Length += data.Length; 
            return this;
        }

        public IBitcoinWriter Add(long v)
        {
            Length += sizeof(long);
            return this;
        }

        public IBitcoinWriter Add(ulong v)
        {
            Length += sizeof(ulong); 
            return this;
        }
        public IBitcoinWriter Add(int v)
        {
            Length += sizeof(int);
            return this;
        }

        public IBitcoinWriter Add(uint v)
        {
            Length += sizeof(uint); 
            return this;
        }
        
        public IBitcoinWriter Add(byte v)
        {
            Length += sizeof(byte); 
            return this;
        }

        public IBitcoinWriter Add(UInt160 v)
        {
            Length += UInt160.Length; 
            return this;
        }

        public IBitcoinWriter Add(UInt256 v)
        {
            Length += UInt256.Length; 
            return this;
        }

        public IBitcoinWriter Add(UInt512 v)
        {
            Length += UInt256.Length; 
            return this;
        } 
    }
}

#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Buffers;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Persistence
{
    public class ByteDataWriter : IDataWriter
    {
        private readonly ArrayBufferWriter<byte> _writer;

        public int Length => _writer.WrittenCount;

        public ReadOnlyByteMemory Memory => _writer.WrittenMemory;
        public ReadOnlyByteSpan Span => _writer.WrittenSpan;
        public byte[] ToArray() => Span.ToArray();

        /// <summary>
        /// ByteDataWriter default constructor
        /// </summary>
        public ByteDataWriter()
        {
            _writer = new ArrayBufferWriter<byte>();
        }

        /// <summary>
        /// ByteDataWriter constructor.
        /// </summary>
        /// <param name="bytes"></param>
        public ByteDataWriter(byte[] bytes)
        {
            _writer =new ArrayBufferWriter<byte>(bytes.Length);
            _writer.Write(bytes);
        }
        
        public IDataWriter Write(byte[] data)
        {
            _writer.Write(data);
            return this;
        }

        public IDataWriter Write(byte v)
        {
            ByteSpan span = stackalloc byte[1] {v}; 
            _writer.Write(span);
            return this;
        }

        public IDataWriter Write(int v)
        {
            _writer.Write(v.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(uint v)
        {
            _writer.Write(v.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(long v)
        {
            _writer.Write(v.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(ulong v)
        {
            _writer.Write(v.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(UInt160 v)
        {
            _writer.Write(v.Span);
            return this;
        }

        public IDataWriter Write(UInt256 v)
        {
            _writer.Write(v.Span);
            return this;
        }

        public IDataWriter Write(UInt512 v)
        {
            _writer.Write(v.Span);
            return this;
        }
    }
}

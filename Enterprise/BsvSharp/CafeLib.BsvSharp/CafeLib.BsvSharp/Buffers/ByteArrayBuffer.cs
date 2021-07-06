#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace CafeLib.BsvSharp.Buffers
{
    public class ByteArrayBuffer : IEnumerable<byte>
    {
        private readonly ArrayBufferWriter<byte> _data;

        public int Length => _data.WrittenCount;

        public ReadOnlyByteMemory Memory => _data.WrittenMemory;
        public ReadOnlyByteSpan Span => _data.WrittenSpan;

        /// <summary>
        /// ByteDataWriter default constructor
        /// </summary>
        public ByteArrayBuffer()
        {
            _data = new ArrayBufferWriter<byte>();
        }

        /// <summary>
        /// ByteDataWriter constructor.
        /// </summary>
        /// <param name="bytes"></param>
        public ByteArrayBuffer(byte[] bytes)
            : this()
        {
            _data.Write(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() => Span;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public ReadOnlyByteSpan Slice(int start, int length) => Span.Slice(start, length);

        /// <summary>
        /// Enumerates the ByteArrayBuffer;
        /// </summary>
        /// <returns></returns>
        public IEnumerator<byte> GetEnumerator()
        {
            for (var i = 0; i < Length; ++i)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Add bytes to buffer
        /// </summary>
        /// <param name="bytes"></param>
        public void Add(byte[] bytes) => _data.Write(bytes);

        /// <summary>
        /// Add bytes via span to buffer.
        /// </summary>
        /// <param name="span"></param>
        public void Add(ReadOnlyByteSpan span) => _data.Write(span);

        public byte this[Index index] => Span[index];

        public ReadOnlyByteSpan this[Range range] => Span[range];

        public static implicit operator ByteArrayBuffer(byte[] rhs) => new ByteArrayBuffer(rhs);
        public static implicit operator byte[](ByteArrayBuffer rhs) => rhs.ToArray();
        public static implicit operator ReadOnlyByteSpan(ByteArrayBuffer rhs) => rhs.Span;
    }
}

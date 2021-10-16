using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace CafeLib.Core.Buffers.Arrays
{
    public class ArrayBuffer<T> : IArrayBuffer<T>
    {
        private readonly ArrayBufferWriter<T> _writer;
        
        public int Length => _writer.WrittenCount;

        public ReadOnlyMemory<T> Memory => _writer.WrittenMemory;
        public ReadOnlySpan<T> Span => _writer.WrittenSpan;

        /// <summary>
        /// ByteDataWriter default constructor
        /// </summary>
        public ArrayBuffer()
        {
            _writer = new ArrayBufferWriter<T>();
        }

        /// <summary>
        /// ByteDataWriter constructor.
        /// </summary>
        /// <param name="items"></param>
        public ArrayBuffer(ReadOnlySpan<T> items)
            : this()
        {
            _writer.Write(items);
        }

        /// <summary>
        /// Convert buffer to array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() => Span.ToArray();

        /// <summary>
        ///  Add item to buffer.
        /// </summary>
        /// <param name="item">item</param>
        public void Add(T item) => Add(new[] {item});

        /// <summary>
        /// Add items to buffer
        /// </summary>
        /// <param name="items">readonly span of items</param>
        public void Add(ReadOnlySpan<T> items) => _writer.Write(items);

        /// <summary>
        /// Get buffer slice.
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="length">slice length</param>
        /// <returns></returns>
        public ReadOnlySpan<T> Slice(int start, int length) => Span.Slice(start, length);

        /// <summary>
        /// Buffer indexer.
        /// </summary>
        /// <param name="index"></param>
        public T this[Index index] => Span[index];

        /// <summary>
        /// Buffer slice.
        /// </summary>
        /// <param name="range"></param>
        public ReadOnlySpan<T> this[Range range] => Span[range];
        
        /// <summary>
        /// Enumerates the ByteArrayBuffer;
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
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
    }
}

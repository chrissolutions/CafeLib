using System;
using System.Collections.Generic;

namespace CafeLib.Core.Buffers.Arrays
{
    public interface IArrayBuffer<T> : IEnumerable<T>
    {
        /// <summary>
        /// Get buffer length.
        /// </summary>
        int Length { get; }
        
        /// <summary>
        /// Get the underling buffer memory
        /// </summary>
        ReadOnlyMemory<T> Memory { get; }
        
        /// <summary>
        /// Get the underlying buffer span.
        /// </summary>
        ReadOnlySpan<T> Span { get; }

        /// <summary>
        ///  return buffer as array. 
        /// </summary>
        /// <returns></returns>
        T[] ToArray();

        /// <summary>
        /// Return buffer slice.
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="length">slice length</param>
        /// <returns></returns>
        ReadOnlySpan<T> Slice(int start, int length);

        /// <summary>
        /// Add item to buffer
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);
        
        /// <summary>
        /// Add bytes to buffer
        /// </summary>
        /// <param name="items"></param>
        void Add(ReadOnlySpan<T> items);

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index"></param>
        T this[Index index] { get; }

        /// <summary>
        /// Range indexer. 
        /// </summary>
        /// <param name="range">range slice</param>
        ReadOnlySpan<T> this[Range range] { get; }
    }
}

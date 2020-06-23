using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Collections.Heaps;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Collections.Queues
{
    public class PriorityQueue<T> : IQueue<T>
    {
        private readonly BinaryHeap<QueueEntry<T>> _heap;
        private readonly object _mutex = new object();

        public bool IsFifo => _heap.IsMaxHeap;

        public PriorityQueue()
        {
            _heap = new BinaryHeap<QueueEntry<T>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value) => Enqueue(value, 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="priority"></param>
        public void Enqueue(T value, int priority)
        {
            lock (_mutex)
            {
                _heap.Add(new QueueEntry<T>(value, priority));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            lock (_mutex)
            {
                return _heap.Remove().Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryDequeue(out T result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryPeek(out T result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the count of items in the priority queue.
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Returns the maximum value in the heap or throws and exception if the heap is empty.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _heap.Peek().Value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _heap.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Queueing.Priority
{
    public class PriorityQueue<T> : IEnumerable<T>  where T : IComparable<T>
    {
        private readonly BinaryHeap<QueueEntry<T>> _heap;
        private readonly object _mutex = new object();

        public bool IsFifo => _heap.IsMaxHeap;

        public PriorityQueue()
        {
            _heap = new BinaryHeap<QueueEntry<T>>();
        }

        public void Enqueue(T value, int priority = 0)
        {
            lock (_mutex)
            {
                _heap.Add(new QueueEntry<T>(value, priority));
            }
        }

        public T Dequeue()
        {
            lock (_mutex)
            {
                return _heap.Remove().Value;
            }
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

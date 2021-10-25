using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Collections
{
    public class PriorityQueue<T> : IQueue<T>
    {
        private readonly BinaryHeap<QueueEntry<T>> _heap;

        public bool IsFifo => _heap.IsMaxHeap;

        public PriorityQueue()
        {
            _heap = new BinaryHeap<QueueEntry<T>>();
        }

        /// <summary>
        /// Add item to queue
        /// </summary>
        /// <param name="item">item to enqueue</param>
        public void Enqueue(T item) => Enqueue(item, 0);

        /// <summary>
        /// Add item and priority value to queue
        /// </summary>
        /// <param name="item">item to enqueue</param>
        /// <param name="priority">priority value</param>
        public void Enqueue(T item, int priority)
        {
            _heap.Add(new QueueEntry<T>(item, priority));
        }

        /// <summary>
        /// Clear priority queue.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            return _heap.Remove().Value;
        }

        /// <summary>
        /// Try dequeuing an item.
        /// </summary>
        /// <param name="item">result item from queue</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool TryDequeue(out T item)
        {
            try
            {
                item = Dequeue();
                return true;
            }
            catch
            {
                item = default;
                return false;
            }
        }

        /// <summary>
        /// Try peeking the queue.
        /// </summary>
        /// <param name="item">result item from queue</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool TryPeek(out T item)
        {
            try
            {
                item = Peek();
                return true;
            }
            catch
            {
                item = default;
                return false;
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

        /// <summary>
        /// Get the enumerator of the priority queue.
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _heap.Select(x => x.Value).GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator of the priority queue.
        /// </summary>
        /// <returns>enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

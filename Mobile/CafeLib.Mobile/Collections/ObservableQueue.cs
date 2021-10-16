using System.Collections.Generic;
using System.Collections.ObjectModel;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Collections
{
    public class ObservableQueue<T> : ObservableCollection<T>
    {
        public uint Limit { get; }

        /// <summary>
        /// ObservableQueue constructor.
        /// </summary>
        /// <param name="limit">queue size limit.</param>
        public ObservableQueue(uint limit = uint.MaxValue)
        {
            Limit = limit;
        }

        /// <summary>
        /// ObservableQueue constructor.
        /// </summary>
        /// <param name="collection">collection of items</param>
        /// <param name="limit">queue size limit.</param>
        public ObservableQueue(IEnumerable<T> collection, uint limit = uint.MaxValue)
            : this(limit)
        {
            foreach (var item in collection)
            {
                if (Count < limit)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Remove an item from the queue.
        /// </summary>
        /// <returns>item</returns>
        public virtual T Dequeue()
        {
            CheckReentrancy();
            var item = base[0];
            RemoveItem(0);
            return item;
        }

        /// <summary>
        /// Place an item onto the queue.
        /// </summary>
        /// <param name="item">item</param>
        public virtual void Enqueue(T item)
        {
            CheckReentrancy();
            lock (this)
            {
                while (Count >= Limit && TryDequeue(out _))
                {
                    // continue.
                }
            }

            Add(item);
        }

        /// <summary>
        /// Remove an item from the queue.
        /// </summary>
        /// <param name="item">item</param>
        /// <returns></returns>
        protected bool TryDequeue(out T item)
        {
            CheckReentrancy();
            item = default;
            if (Count == 0) return false;
            item = Dequeue();
            return true;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable InvertIf
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Collections.Heaps
{
    public class BinaryHeap<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Member_Variables

        private readonly List<T> _list;
        private readonly int _comparison;

        #endregion

        #region Constructors

        /// <summary>
        /// Binary heap constructor.
        /// </summary>
        /// <param name="capacity">initial heap capacity</param>
        /// <param name="maxHeap">determines min/max heap behavior; max heap is default.</param>
        public BinaryHeap(int capacity = 0, bool maxHeap = true)
        {
            _comparison = maxHeap ? 1 : -1;
            _list = (capacity > 0)
                ? new List<T>(capacity)
                : new List<T>();

            // Add placeholder in top position.
            _list.Add(default);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of entries in the heap.
        /// </summary>
        public int Count => _list.Count - 1;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Determines whether the heap is a max-heap.
        /// </summary>
        public bool IsMaxHeap => _comparison == 1;

        #endregion

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            return _list.Skip(1).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        /// <summary>
        /// Add keyed item into the binary heap and allow duplicates.
        /// Allow duplicates.
        /// </summary>
        /// <param name="item">item to add to heap.</param>
        public void Add(T item)
        {
            int slot = _list.Count;
            _list.Add(item);

            // Percolate up
            while (slot > 1 && _comparison * item.CompareTo(_list[slot/2]) > 0)
            {
                _list[slot] = _list[slot/2];
                slot /= 2;
            }

            _list[slot] = item;
        }

        /// <summary>
        /// Clears binary heap.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _list.Add(default);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the top-level value in the heap or throws and exception if the heap is empty.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (this.Any())
            {
                return _list[1];
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Removes the bottom item from the binary heap.
        /// </summary>
        /// <returns>value</returns>
        public T Remove()
        {
            if (!this.Any())
                return default;

            var topItem = _list[1];
            _list[1] = _list[Count];
            _list.RemoveAt(Count);
            TrickleDown(1);
            return topItem;
        }


        #endregion

        #region Helpers

        private void TrickleDown(int slot)
        {
            if (Count != 0)
            {
                T tmp = _list[slot];

                while (slot * 2 <= Count)
                {
                    int child = slot * 2;

                    if (child != Count && _comparison * _list[child + 1].CompareTo(_list[child]) > 0)
                        child++;

                    if (_comparison * _list[child].CompareTo(tmp) <= 0)
                        break;

                    _list[slot] = _list[child];
                    slot = child;
                }

                _list[slot] = tmp;
            }
        }

        #endregion
    }
}

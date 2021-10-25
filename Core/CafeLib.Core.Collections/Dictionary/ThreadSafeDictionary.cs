using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.Collections
{
    public class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly List<KeyValuePair<TKey, TValue>> _itemList;
        private readonly Dictionary<TKey, TValue> _itemMap;
        private readonly ReaderWriterLockSlim _mutex;

        /// <summary>
        /// Creates a dictionary with its default capacity and key comparer.
        /// </summary>
        public ThreadSafeDictionary()
        {
            _itemList = new List<KeyValuePair<TKey, TValue>>();
            _itemMap = new Dictionary<TKey, TValue>();
            _mutex = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Obtain the number of items in the dictionary.
        /// </summary>
        public int Count => _itemList.Count;

        /// <summary>
        /// Determines whether the collection is read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Obtain collection of the keys in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys => _itemList.Select(x => x.Key).ToArray();

        /// <summary>
        /// Obtain collection of the values in the dictionary.
        /// </summary>
        public ICollection<TValue> Values => _itemList.Select(x => x.Value).ToArray();


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _itemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a key value pair to the dictionary.
        /// </summary>
        /// <param name="item">key/value pair</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public void Clear()
        {
            _mutex.EnterWriteLock();
            try
            {
                _itemList.Clear();
                _itemMap.Clear();
            }
            finally
            {
                _mutex.ExitWriteLock();
            }
        }

        /// <summary>
        /// Determines whether the key/value pair is contained in the dictionary.
        /// </summary>
        /// <param name="item">key/value pair</param>
        /// <returns>true if contained in the dictionary; otherwise false</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _itemList.Contains(item);
        }

        /// <summary>
        /// Copy the key-value pairs to an array.
        /// </summary>
        /// <param name="array">key-value array</param>
        /// <param name="arrayIndex">start index</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _itemList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove a key-value item.
        /// </summary>
        /// <param name="item">key-value pair</param>
        /// <returns>true if removed; false otherwise</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Add a key and value items to the dictionary.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _mutex.EnterWriteLock();
            try
            {
                _itemMap.Add(key, value);
                _itemList.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
            finally
            {
                _mutex.ExitWriteLock();
            }
        }

        /// <summary>
        /// Determines if the key exist in the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns> true if the key exists in the dictionary, false otherwise.</returns>
        public bool ContainsKey(TKey key)
        {
            return _itemMap.ContainsKey(key);
        }

        /// <summary>
        /// Remove the item from the dictionary.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns> true if removed, false otherwise.</returns>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _mutex.EnterWriteLock();
            try
            {
                var index = IndexOfKey(key);
                if (index >= 0)
                {
                    _itemList.RemoveAt(index);
                }

                return _itemMap.Remove(key);
            }
            finally
            {
                _mutex.ExitWriteLock();
            }
        }

        /// <summary>
        /// Try to get the value based on the key.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">output value</param>
        /// <returns>true if retrieved, false otherwise</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            try
            {
                value = this[key];
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Obtain value through indexer.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        public TValue this[TKey key]
        {
            get => _itemMap[key];
            set
            {
                _mutex.EnterWriteLock();
                try
                {
                    _itemMap[key] = value;
                    _itemList[IndexOfKey(key)] = new KeyValuePair<TKey,TValue>(key, value);
                }
                finally
                {
                    _mutex.ExitWriteLock();
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Obtain the index associated with the key.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>index</returns>
        private int IndexOfKey(TKey key)
        {
            var index = -1;
            var result = _itemList.Some((x, i) => 
            {
                index = i;
                return x.Key.Equals(key);
            });

            return result ? index : -1;
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.Collections
{
	/// <summary>
	/// Represents a generic collection of key/value pairs that are ordered independently of the key and value.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
	public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
	{
		private IDictionary<TKey, TValue> _dictionary;
		private List<KeyValuePair<TKey, TValue>> _list;
		private readonly IEqualityComparer<TKey> _comparer;

        /// <summary>
        /// Gets the dictionary object that stores the keys and values
        /// </summary>
        /// <value>The dictionary object that stores the keys and values for the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></value>
        /// <remarks>Accessing this property will create the dictionary object if necessary</remarks>
        private IDictionary<TKey, TValue> Dictionary => _dictionary ??= new ConcurrentDictionary<TKey, TValue>(_comparer);

        /// <summary>
        /// Gets the list object that stores the key/value pairs.
        /// </summary>
        /// <value>The list object that stores the key/value pairs for the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></value>
        /// <remarks>Accessing this property will create the list object if necessary.</remarks>
        private List<KeyValuePair<TKey, TValue>> List => _list ??= new List<KeyValuePair<TKey, TValue>>();

        public int Count => List.Count;
        public bool IsReadOnly => Dictionary.IsReadOnly;
        public ICollection<TKey> Keys => Dictionary.Keys;
        public ICollection<TValue> Values => Dictionary.Values;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public OrderedDictionary()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}">IEqualityComparer&lt;TKey&gt;</see> to use when comparing keys, or <null/> to use the default <see cref="EqualityComparer{TKey}">EqualityComparer&lt;TKey&gt;</see> for the type of the key.</param>
        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>
        /// Adds an entry with the specified key and value into the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. This value can be <null/>.</param>
        /// <returns>The index of the newly added entry</returns>
        /// <remarks>A key cannot be <null/>, but a value can be.
        /// <para>You can also use the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)"/> property to add new elements by setting the value of a key that does not exist in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection; however, if the specified key already exists in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>, setting the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)"/> property overwrites the old value. In contrast, the <see cref="M:Add"/> method does not modify existing elements.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <null/></exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></exception>
        public int Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
            List.Add(new KeyValuePair<TKey, TValue>(key, value));
            return Count - 1;
        }
        /// <summary>
        /// Adds the specified value to the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> with the specified key.
        /// </summary>
        /// <param name="item">The <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see> structure representing the key and value to add to the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <remarks>The capacity is not changed as a result of calling this method.</remarks>
        public void Clear()
        {
            Dictionary.Clear();
            List.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => Dictionary.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> to an array of type <see cref="T:KeyValuePair`2>"/>, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of type <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see> that is the destination of the <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see> elements copied from the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes a key and value from the dictionary.
        /// </summary>
        /// <param name="item">The <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see> structure representing the key and value to remove from the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.</param>
        /// <returns><see langword="true"/> if the key and value is successfully found and removed; otherwise, <see langword="false"/>. This method returns <see langword="false"/> if item is not found in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
            List.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Inserts a new entry into the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection with the specified key and value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. The value can be <null/> if the type of the values in the dictionary is a reference type.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.<br/>
        /// -or-<br/>
        /// <paramref name="index"/> is greater than <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <null/>.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.</exception>
        public void Insert(int index, TKey key, TValue value)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Dictionary.Add(key, value);
            List.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Removes the entry at the specified index from the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.<br/>
        /// -or-<br/>
        /// index is equal to or greater than <see cref="Count"/>.</exception>
        public void RemoveAt(int index)
        {
            if (index >= Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "'index' must be non-negative and less than the size of the collection");

            var key = List[index].Key;
            List.RemoveAt(index);
            Dictionary.Remove(key);
        }


        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <value>The value of the item at the specified index.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.<br/>
        /// -or-<br/>
        /// index is equal to or greater than <see cref="Count"/>.</exception>
        public TValue this[int index]
        {
            get => List[index].Value;

            set
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be non-negative and less than the size of the collection");

                var key = List[index].Key;
                List[index] = new KeyValuePair<TKey, TValue>(key, value);
                Dictionary.AddOrUpdate(key, value, (k, v) => value);
            }
        }

        /// <summary>
        /// Adds an entry with the specified key and value into the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. This value can be <null/>.</param>
        /// <returns>The index of the newly added entry</returns>
        /// <remarks>A key cannot be <null/>, but a value can be.
        /// <para>You can also use the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)"/> property to add new elements by setting the value of a key that does not exist in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection; however, if the specified key already exists in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>, setting the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)"/> property overwrites the old value. In contrast, the <see cref="M:Add"/> method does not modify existing elements.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <null/></exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></exception>
        int IOrderedDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
            List.Add(new KeyValuePair<TKey, TValue>(key, value));
            return Count - 1;
        }

        /// <summary>
        /// Determines whether the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.</param>
        /// <returns><see langword="true"/> if the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <null/></exception>
        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

        /// <summary>
        /// Removes the entry with the specified key from the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <returns><see langword="true"/> if the key was found and the corresponding element was removed; otherwise, <see langword="false"/></returns>
        public bool Remove(TKey key)
        {
            if (null == key)
                throw new ArgumentNullException(nameof(key));

            int index = IndexOfKey(key);
            if (index >= 0)
            {
                if (Dictionary.Remove(key))
                {
                    List.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }

        /// <summary>
        /// Returns the zero-based index of the specified key in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></param>
        /// <returns>The zero-based index of <paramref name="key"/>, if <paramref name="key"/> is found in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>; otherwise, -1</returns>
        /// <remarks>This method performs a linear search; therefore it has a cost of O(n) at worst.</remarks>
        private int IndexOfKey(TKey key)
        {
            if (null == key)
                throw new ArgumentNullException(nameof(key));

            for (int index = 0; index < List.Count; index++)
            {
                KeyValuePair<TKey, TValue> entry = List[index];
                TKey next = entry.Key;
                if (null != _comparer)
                {
                    if (_comparer.Equals(next, key))
                    {
                        return index;
                    }
                }
                else if (next.Equals(key))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}

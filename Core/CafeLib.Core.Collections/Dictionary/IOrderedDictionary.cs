using System;
using System.Collections.Generic;

namespace CafeLib.Core.Collections
{
	/// <summary>
	/// Represents a generic collection of key/value pairs that are ordered independently of the key and value.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
	public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		/// <summary>
		/// Adds an entry with the specified key and value into the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> collection with the lowest available index.
		/// </summary>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add.</param>
		/// <returns>The index of the newly added entry</returns>
		/// <remarks>
		/// <para>You can also use the <see cref="P:System.Collections.Generic.IDictionary{TKey,TValue}.Item(TKey)"/> property to add new elements by setting the value of a key that does not exist in the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> collection; however, if the specified key already exists in the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see>, setting the <see cref="P:Item(TKey)"/> property overwrites the old value. In contrast, the <see cref="M:Add"/> method does not modify existing elements.</para></remarks>
		/// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see></exception>
		/// <exception cref="NotSupportedException">The <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> is read-only.<br/>
		/// -or-<br/>
		/// The <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> has a fized size.</exception>
		new int Add(TKey key, TValue value);

        /// <summary>
        /// Inserts a new entry into the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> collection with the specified key and value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. The value can be <null/> if the type of the values in the dictionary is a reference type.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.<br/>
        /// -or-<br/>
        /// <paramref name="index"/> is greater than <see cref="System.Collections.ICollection.Count"/>.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see>.</exception>
        /// <exception cref="NotSupportedException">The <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> is read-only.<br/>
        /// -or-<br/>
        /// The <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see> has a fized size.</exception>
        void Insert(int index, TKey key, TValue value);

        /// <summary>Removes the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///         <paramref name="index" /> is less than 0.
        /// -or-
        /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ICollection.Count" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Specialized.IOrderedDictionary" /> collection is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.Specialized.IOrderedDictionary" /> collection has a fixed size.</exception>
        void RemoveAt(int index);

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///         <paramref name="index" /> is less than 0.
        /// -or-
        /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ICollection.Count" />.</exception>
        TValue this[int index] { get; set; }
	}
}

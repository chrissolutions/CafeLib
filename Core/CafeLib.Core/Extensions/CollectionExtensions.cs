using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;

namespace CafeLib.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="enumerable">enumerable</param>
        /// <param name="eachFunc">select function</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> eachFunc)
        {
            foreach (var item in enumerable)
            {
                eachFunc?.Invoke(item);
            }
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <typeparam name="TU">result type</typeparam>
        /// <param name="enumerable">enumerable</param>
        /// <param name="eachFunc">select function</param>
        /// <returns>returns list of results</returns>
        public static IEnumerable<TU> ForEach<T, TU>(this IEnumerable<T> enumerable, Func<T, TU> eachFunc)
        {
            return enumerable.Select(eachFunc.Invoke).ToList();
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <typeparam name="TU">result type</typeparam>
        /// <param name="enumerable">enumerable</param>
        /// <param name="eachFunc">select function with item and index</param>
        /// <returns>returns list of results</returns>
        public static IEnumerable<TU> ForEach<T, TU>(this IEnumerable<T> enumerable, Func<T, int, TU> eachFunc)
        {
            var index = 0;
            return enumerable.Select(item => eachFunc.Invoke(item, index++)).ToList();
        }

        /// <summary>
        /// Some extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="enumerable">enumerable</param>
        /// <param name="someFunc">some function with item and index</param>
        /// <returns>returns list of results</returns>
        public static bool Some<T>(this IEnumerable<T> enumerable, Func<T, int, bool> someFunc)
        {
            var index = 0;
            return enumerable.Any(item => someFunc.Invoke(item, index++));
        }

        /// <summary>
        /// Add value to the dictionary.
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
        {
            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retreive or add a value to dictionary
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="value">value</param>
        /// <returns>value</returns>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }

            return dictionary[key];
        }

        /// <summary>
        /// Retreive or add a value to dictionary via add function.
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="addFunc">add function returning the value</param>
        /// <returns>value</returns>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> addFunc)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, addFunc());
            }

            return dictionary[key];
        }

        /// <summary>
        /// Add source dictionary items to the target dictionary.
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void AddRange<TK, TV>(this IDictionary<TK, TV> target, NonNullable<IDictionary<TK, TV>> source)
        {
            foreach (var item in source.Value)
            {
                target.AddOrUpdate(item.Key, item.Value, (k, v) => item.Value);
            }
        }

        /// <summary>
        /// Add or update a key/value pair of the dictionary.  
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="value">value</param>
        /// <param name="updateFunc">update function</param>
        /// <returns>The value for the key.</returns>
        /// <remarks>
        /// Add or updates the key/value pair to the dictionary.
        /// </remarks>
        public static TV AddOrUpdate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value, Func<TK, TV, TV> updateFunc)
        {
            return dictionary.AddOrUpdate(key, k => dictionary.GetOrAdd(k, value), updateFunc);
        }

        /// <summary>
        /// Add or update a key/value pair of the dictionary.  
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="addFunc">add function</param>
        /// <param name="updateFunc">update function</param>
        /// <returns></returns>
        public static TV AddOrUpdate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> addFunc, Func<TK, TV, TV> updateFunc)
        {
            return AddOrUpdateInternal(dictionary, key, new NonNullable<Func<TK, TV>>(addFunc), new NonNullable<Func<TK, TV, TV>>(updateFunc));
        }

        /// <summary>
        /// Add value to the dictionary.
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static object ToObject<T>(this IDictionary<string, T> dictionary)
        {
            try
            {
                dynamic dyn = dictionary.Aggregate(new ExpandoObject() as IDictionary<string, object>, (x, p) => { x.Add(p.Key.ToString(), p.Value); return x; });
                return (object) dyn;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Add range of items to observable collection.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">observable collection</param>
        /// <param name="items">item to add to the collection</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Finds the index of the first item matching an expression in a collection.
        /// </summary>
        ///<param name="collection">The collection to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this ICollection<T> collection, Predicate<T> predicate)
        {
            return FindIndexInternal(collection, new NonNullable<Predicate<T>>(predicate));
        }

        ///<summary>
        /// Finds the index of the first occurrence of an item in a collection.
        /// </summary>
        ///<param name="collection">The collection to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this ICollection<T> collection, T item)
        {
            return collection.FindIndex(x => EqualityComparer<T>.Default.Equals(item, x));
        }

        #region Helpers

        /// <summary>
        /// Adds a key/value pair to the dictionary.  
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="addFunc">add function</param>
        /// <param name="updateFunc">update function</param>
        /// <returns>The new value for the key.</returns>
        /// <remarks>
        /// Add or updates the key/value pair to the dictionary.
        /// </remarks>
        private static TV AddOrUpdateInternal<TK, TV>(IDictionary<TK, TV> dictionary, TK key, NonNullable<Func<TK, TV>> addFunc, NonNullable<Func<TK, TV, TV>> updateFunc)
        {
            dictionary[key] = !dictionary.ContainsKey(key)
                ? addFunc.Value.Invoke(key)
                : updateFunc.Value.Invoke(key, dictionary[key]);

            return dictionary[key];
        }

        /// <summary>
        /// Finds the index of the first item matching an expression in a collection.
        /// </summary>
        ///<param name="collection">The collection to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndexInternal<T>(ICollection<T> collection, NonNullable<Predicate<T>> predicate)
        {
            var result = 0;
            foreach (var item in collection)
            {
                if (predicate.Value.Invoke(item)) return result;
                ++result;
            }

            return -1;
        }

        #endregion
    }
}


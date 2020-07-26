using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.Support;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="action">iterative action</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action?.Invoke(item);
            }
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="action">iterative action</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in collection)
            {
                action?.Invoke(item, index++);
            }
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <typeparam name="TU">result type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="func">select function</param>
        /// <returns>returns list of results</returns>
        public static IEnumerable<TU> ForEach<T, TU>(this IEnumerable<T> collection, Func<T, TU> func)
        {
            return collection.Select(func.Invoke).ToList();
        }

        /// <summary>
        /// For each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <typeparam name="TU">result type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="func">select function with item and index</param>
        /// <returns>returns list of results</returns>
        public static IEnumerable<TU> ForEach<T, TU>(this IEnumerable<T> collection, Func<T, int, TU> func)
        {
            return collection.Select(func.Invoke).ToList();
        }

        /// <summary>
        /// Asynchronous for each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="action">iterative action</param>
        public static Task ForEachAsync<T>(this IEnumerable<T> collection, Action<T> action)
        {
            return collection.ForEachAsync(async x => { action.Invoke(x); await Task.CompletedTask; });
        }

        /// <summary>
        /// Asynchronous for each extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="task">iterative task</param>
        public static Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> task)
        {
            var tasks = collection.Select(task).ToArray();
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Filter collection by distinct items.
        /// </summary>
        /// <typeparam name="T">collection type</typeparam>
        /// <typeparam name="TKey">filter key type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="filter">filter</param>
        /// <returns>collection of distinct items</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> filter)
        {
            return collection.GroupBy(filter).Select(x => x.First());
        }

        /// <summary>
        /// Obtain groups of non-unique items in collection.
        /// </summary>
        /// <typeparam name="T">collection type</typeparam>
        /// <typeparam name="TKey">filter key type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="filter">filter</param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, T>> NonUnique<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> filter)
        {
            return collection.GroupBy(filter).Where(x => x.Count() > 1);
        }

        /// <summary>
        /// Every test whether all elements matches the predicate
        /// </summary>
        /// <typeparam name="T">collection item type</typeparam>
        /// <param name="collection">collection of item type</param>
        /// <param name="predicate">predicate function</param>
        /// <returns>returns true if all items match the predicate; false otherwise</returns>
        public static bool Every<T>(this IEnumerable<T> collection, Func<T, int, bool> predicate)
        {
            var index = 0;
            return collection.All(item => predicate.Invoke(item, index++));
        }

        /// <summary>
        /// Some test whether any element matches the predicate.
        /// </summary>
        /// <typeparam name="T">collection item type</typeparam>
        /// <param name="collection">collection of item type</param>
        /// <param name="predicate">predicate function</param>
        /// <returns>returns true if any item matches the predicate; false otherwise</returns>
        public static bool Some<T>(this IEnumerable<T> collection, Func<T, int, bool> predicate)
        {
            var index = 0;
            return collection.Any(item => predicate.Invoke(item, index++));
        }

        /// <summary>
        /// Transform collection items.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="eachFunc">select function with item</param>
        /// <returns>returns list of results</returns>
        public static IEnumerable<T> Transform<T>(this IEnumerable<T> collection, Action<T> eachFunc)
        {
            return collection.Select(item =>
            {
                eachFunc.Invoke(item);
                return item;
            }).ToList();
        }

        /// <summary>
        /// Traversal extension.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="subtree">subtree</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> collection, Func<T, IEnumerable<T>> subtree)
        {
            var stack = new Stack<T>(collection);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var item in subtree(next))
                    stack.Push(item);
            }
        }

        /// <summary>
        /// Return a unique collection filtered by a key.
        /// </summary>
        /// <typeparam name="T">collection type</typeparam>
        /// <typeparam name="TKey">filter key type</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="filter">filter</param>
        /// <returns>unique collection</returns>
        public static IEnumerable<T> Unique<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> filter)
        {
            return collection.GroupBy(filter).Where(x => x.Count() == 1).Select(x => x.First());
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
        /// Retrieve or add a value to dictionary
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
        /// Retrieve or add a value to dictionary via add function.
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
        /// Convert dictionary to an object.
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="dictionary"></param>
        /// <returns>object</returns>
        public static T ToObject<T>(this IDictionary<string, T> dictionary)
        {
            try
            {
                dynamic dyn = dictionary.Aggregate(new ExpandoObject() as IDictionary<string, object>, (x, p) => { x.Add(p.Key.ToString(), p.Value); return x; });
                return (T) (object) dyn;
            }
            catch (Exception)
            {
                return default;
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
        private static int FindIndexInternal<T>(IEnumerable<T> collection, NonNullable<Predicate<T>> predicate)
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


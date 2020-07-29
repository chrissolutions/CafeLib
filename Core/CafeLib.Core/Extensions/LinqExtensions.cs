using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class LinqExtensions
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
    }
}


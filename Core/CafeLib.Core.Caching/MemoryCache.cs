using System;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using SystemMemoryCache = System.Runtime.Caching.MemoryCache;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Caching
{
    public class MemoryCache : ICacheService, IDisposable
    {
        private readonly SystemMemoryCache _cache;
        private readonly int _lifetimeMilliseconds;

        private static readonly int DefaultLifetimeMinutes = (int)TimeSpan.FromMinutes(15).TotalMilliseconds;

        /// <summary>
        /// 
        /// </summary>
        public MemoryCache()
        {
            _cache = new SystemMemoryCache(Guid.NewGuid().ToString());
            _lifetimeMilliseconds = DefaultLifetimeMinutes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lifetimeMilliseconds"></param>
        public MemoryCache(int lifetimeMilliseconds)
            : this()
        {
            _lifetimeMilliseconds = lifetimeMilliseconds > 0 ? lifetimeMilliseconds : DefaultLifetimeMinutes;
        }

        /// <summary>
        /// Get cached item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(NonNullable<string> key) where T : class
        {
            return (T)_cache.Get(CacheKey<T>(key));
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <returns>item</returns>
        public T Get<T>(NonNullable<string> key, Func<T> getItem) where T : class
        {
            return Get(key, getItem, false);
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <param name="update">if true force item update via getItem, otherwise default behavior</param>
        /// <returns>item</returns>
        public T Get<T>(NonNullable<string> key, Func<T> getItem, bool update) where T : class
        {
            if (!update && _cache.Get(CacheKey<T>(key)) is T item) return item;

            item = getItem();
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMilliseconds(_lifetimeMilliseconds) };
            _cache.Add(CacheItem(key, item), cacheItemPolicy);
            return item;
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <returns>item</returns>
        public Task<T> GetAsync<T>(NonNullable<string> key, Func<T> getItem) where T : class
            => GetAsync(key, Task.FromResult(getItem()));

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <param name="update">if true force item update via getItem, otherwise default behavior</param>
        /// <returns>item</returns>
        public Task<T> GetAsync<T>(NonNullable<string> key, Func<T> getItem, bool update) where T : class
            => GetAsync(key, Task.FromResult(getItem()), update);

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item task</param>
        /// <returns>item</returns>
        public Task<T> GetAsync<T>(NonNullable<string> key, Task<T> getItem) where T : class
            => GetAsync(key, getItem, false);

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item task</param>
        /// <param name="update">if true force item update via getItem, otherwise default behavior</param>
        /// <returns>item</returns>
        public async Task<T> GetAsync<T>(NonNullable<string> key, Task<T> getItem, bool update) where T : class
        {
            if (!update && _cache.Get(CacheKey<T>(key)) is T item) return item;

            item = await getItem.ConfigureAwait(false);
            _cache.Add(CacheItem(key, item), new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMilliseconds(_lifetimeMilliseconds) });
            return item;
        }

        /// <summary>
        /// Clear cache.
        /// </summary>
        public void Clear()
        {
            _cache.Dispose();
        }

        /// <summary>
        /// Disposes the cache.
        /// </summary>
        public void Dispose()
        {
            _cache.Dispose();
        }

        /// <summary>
        /// Determine whether the cache key exists.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <returns>true if the key exists otherwise false</returns>
        public bool Contains<T>(NonNullable<string> key) where T : class
        {
            return _cache.Contains(CacheKey<T>(key));
        }

        /// <summary>
        /// Set a cache item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="item">item</param>
        /// <returns>true if added to the cache otherwise false</returns>
        public bool Set<T>(NonNullable<string> key, T item) where T : class
        {
            var contains = Contains<T>(key);

            if (!contains)
            {
                _cache.Set(CacheItem(key, item), new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMilliseconds(_lifetimeMilliseconds) });
            }

            return !contains;
        }

        /// <summary>
        /// Set an item in the cache.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="item">item</param>
        /// <param name="update">if true force item set of the item, otherwise default behavior</param>
        /// <returns>true if added to the cache otherwise false</returns>
        public bool Set<T>(NonNullable<string> key, T item, bool update) where T : class
        {
            if (update)
            {
                _cache.Remove(CacheKey<T>(key), typeof(T).AssemblyQualifiedName);
            }

            return Set(CacheKey<T>(key), item);
        }

        /// <summary>
        /// Construct a cache item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CacheItem CacheItem<T>(NonNullable<string> key, T item) where T : class
        {
            return new CacheItem(CacheKey<T>(key), item);
        }

        /// <summary>
        /// Construct a cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string CacheKey<T>(NonNullable<string> key) where T : class
        {
            return $"{typeof(T).AssemblyQualifiedName}.{key}";
        }
    }
}
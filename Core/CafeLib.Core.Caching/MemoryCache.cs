using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using SystemMemoryCache = System.Runtime.Caching.MemoryCache;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Caching
{
    public class MemoryCache : ICacheService, IDisposable
    {
        private static readonly SystemMemoryCache _cache = SystemMemoryCache.Default;

        private const int DefaultLifetimeMinutes = 15;
        private readonly int _lifetimeMilliseconds;

        /// <summary>
        /// 
        /// </summary>
        public MemoryCache()
        {
            _lifetimeMilliseconds = (int)TimeSpan.FromMinutes(DefaultLifetimeMinutes).TotalMilliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lifetimeMilliseconds"></param>
        public MemoryCache(int lifetimeMilliseconds)
        {
            _lifetimeMilliseconds = lifetimeMilliseconds;
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <returns>item</returns>
        public T Get<T>(string key, Func<T> getItem) where T : class
        {
            return Get(key, getItem, false);
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <param name="updateNow">force item update via getItem</param>
        /// <returns>item</returns>
        public T Get<T>(string key, Func<T> getItem, bool updateNow) where T : class
        {
            if (!updateNow && _cache.Get(key) is T item) return item;

            item = getItem();
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMilliseconds(_lifetimeMilliseconds) };
            _cache.Add(key, item, cacheItemPolicy);
            return item;
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <returns>item</returns>
        public Task<T> GetAsync<T>(string key, Func<T> getItem) where T : class
        {
            return GetAsync(key, Task.FromResult(getItem()));
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item function</param>
        /// <param name="updateNow">force item update via getItem</param>
        /// <returns>item</returns>
        public Task<T> GetAsync<T>(string key, Func<T> getItem, bool updateNow) where T : class
        {
            return GetAsync(key, Task.FromResult(getItem()), updateNow);
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item task</param>
        /// <returns>item</returns>
        public Task<T> GetAsync<T>(string key, Task<T> getItem) where T : class
        {
            return GetAsync(key, getItem, false);
        }

        /// <summary>
        /// Get cached item.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="key">item key</param>
        /// <param name="getItem">get item task</param>
        /// <param name="updateNow">force item update via getItem</param>
        /// <returns>item</returns>
        public async Task<T> GetAsync<T>(string key, Task<T> getItem, bool updateNow) where T : class
        {
            if (!updateNow && _cache.Get(key) is T item) return item;

            item = await getItem.ConfigureAwait(false);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMilliseconds(_lifetimeMilliseconds) };
            _cache.Add(key, item, cacheItemPolicy);
            return item;
        }

        /// <summary>
        /// Return a cache key from key name.
        /// </summary>
        /// <typeparam name="T">type to use to construct key</typeparam>
        /// <param name="name">name</param>
        /// <returns>cache key</returns>
        public string MakeCacheKey<T>(string name)
        {
            return $"{typeof(T).AssemblyQualifiedName}.{name}";
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
    }
}
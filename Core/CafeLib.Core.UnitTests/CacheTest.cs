using CafeLib.Core.Caching;
using CafeLib.Core.UnitTests.Caching;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class CacheTest
    {
        [Fact]
        public void SetItemIntoCache()
        {
            using var cache = new MemoryCache(1000);
            var result = cache.Set("myKey", new CacheItem {Value = 25});
            Assert.True(result);
            Assert.Equal(25, cache.Get<CacheItem>("myKey").Value);
        }

        [Fact]
        public void SetItemIntoCache_TryUpdate()
        {
            // Set item in cache.
            using var cache = new MemoryCache();
            var result = cache.Set("myKey", new CacheItem { Value = 25 });
            Assert.True(result);
            Assert.Equal(25, cache.Get<CacheItem>("myKey").Value);

            // Immediately try to set item again.
            result = cache.Set("myKey", new CacheItem { Value = 30 });
            Assert.False(result);

            // Remove from cache.
            cache.Remove<CacheItem>("myKey");

            // Try again.
            result = cache.Set("myKey", new CacheItem { Value = 30 });
            Assert.True(result);
        }

        [Fact]
        public void GetWithGetterCallback()
        {
            using var cache = new MemoryCache(100000);
            var item = cache.Get("myKey", () => new CacheItem { Value = 25 });
            Assert.Equal(item.Value, cache.Get<CacheItem>("myKey").Value);
        }
    }
}

using System;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Core.Caching
{
    public interface ICacheService
    {
        T Get<T>(string key, Func<T> getItem) where T : class;
        T Get<T>(string key, Func<T> getItem, bool forceUpdate) where T : class;

        Task<T> GetAsync<T>(string key, Func<T> getItem) where T : class;
        Task<T> GetAsync<T>(string key, Func<T> getItem, bool forceUpdate) where T : class;

        Task<T> GetAsync<T>(string key, Task<T> getItem) where T : class;
        Task<T> GetAsync<T>(string key, Task<T> getItem, bool forceUpdate) where T : class;

        string MakeCacheKey<T>(string name);

        void Clear();
    }
}

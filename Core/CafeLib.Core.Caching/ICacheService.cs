using CafeLib.Core.Support;
using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Caching
{
    public interface ICacheService
    {
        void Clear();

        bool Contains<T>(NonNullable<string> key) where T : class;

        T Get<T>(NonNullable<string> key) where T : class;
        T Get<T>(NonNullable<string> key, Func<T> getItem) where T : class;
        T Get<T>(NonNullable<string> key, Func<T> getItem, bool update) where T : class;

        Task<T> GetAsync<T>(NonNullable<string> key, Func<T> getItem) where T : class;
        Task<T> GetAsync<T>(NonNullable<string> key, Func<T> getItem, bool update) where T : class;

        Task<T> GetAsync<T>(NonNullable<string> key, Task<T> getItem) where T : class;
        Task<T> GetAsync<T>(NonNullable<string> key, Task<T> getItem, bool update) where T : class;

        bool Set<T>(NonNullable<string> key, T item) where T : class;
        bool Set<T>(NonNullable<string> key, T item, bool update) where T : class;
    }
}

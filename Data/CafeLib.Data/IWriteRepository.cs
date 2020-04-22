using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Sources;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data
{
    public interface IWriteRepository<T> where T : class, IEntity
    {
        Task<T> Add(T entity);

        Task<bool> Add(IEnumerable<T> entities);

        Task<bool> RemoveByKey<TKey>(TKey key);

        Task<int> RemoveByKey<TKey>(IEnumerable<TKey> keys);

        Task<bool> Remove(T entity);

        Task<int> Remove(IEnumerable<T> entities);

        Task<int> Remove(Expression<Func<T, bool>> predicate, object? parameters);

        Task<T> Save(T entity);

        Task<int> Save(IEnumerable<T> entities, params Expression<Func<T, object>>[]? expressions);

        Task<bool> Update(T entity);

        Task<bool> Update(IEnumerable<T> entities);

        Task<SaveResult<TU>> ExecuteSave<TU>(string sql, object? parameters);
    }
}
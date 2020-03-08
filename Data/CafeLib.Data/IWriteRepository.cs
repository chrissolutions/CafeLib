using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Dto;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data
{
    public interface IWriteRepository<T> where T : class, IEntity
    {
        Task<T> Add(T entity);

        Task<bool> Add(IEnumerable<T> entities);

        Task<bool> RemoveById<TU>(TU id);

        Task<bool> RemoveById<TU>(IEnumerable<TU> id);

        Task<bool> Remove(T entity);

        Task<bool> Remove(IEnumerable<T> entities);

        Task<int> Remove(Expression<Func<T, bool>> predicate, params object[]? parameters);

        Task<T> Save(T entity);

        Task<int> Save(IEnumerable<T> entities, params Expression<Func<T, object>>[]? expressions);

        Task<bool> Update(T entity);

        Task<bool> Update(IEnumerable<T> entities);

        Task<SaveResult<TU>> ExecuteSave<TU>(string sql, params object[]? parameters);
    }
}
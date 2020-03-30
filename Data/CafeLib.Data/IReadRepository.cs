using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Sources;

namespace CafeLib.Data
{
    public interface IReadRepository<T> where T : class, IEntity
    {
        Task<bool> Any();

        Task<bool> Any<TU>(TU id);

        Task<bool> Any(Expression<Func<T, bool>> predicate, params object[]? parameters);

        Task<int> Count();

        Task<int> Count(Expression<Func<T, bool>> predicate, params object[]? parameters);

        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, params object[]? parameters);

        Task<IEnumerable<T>> FindAll();

        Task<T> FindOne(Expression<Func<T, bool>> predicate, params object[]? parameters);

        Task<T> FindByKey<TKey>(TKey id);

        Task<IEnumerable<T>> FindByKey<TKey>(IEnumerable<TKey> ids);

        Task<IEnumerable<T>> FindBySqlQuery(string sql, params object[]? parameters);

        Task<QueryResult<T>> ExecuteQuery(string sql, params object[]? parameters);
    }
}
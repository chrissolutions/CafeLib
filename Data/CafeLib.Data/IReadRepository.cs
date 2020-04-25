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

        Task<bool> Any<TKey>(TKey key);

        /// <summary>
        /// Determine whether the entity has any entities.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="predicate">query condition</param>
        /// <returns>
        ///     true: if the entity has entries.
        ///     false: if the entity is empty.
        /// </returns>
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        Task<int> Count();

        Task<int> Count(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> FindAll();

        Task<T> FindOne(Expression<Func<T, bool>> predicate);

        Task<T> FindByKey<TKey>(TKey id);

        Task<IEnumerable<T>> FindByKey<TKey>(IEnumerable<TKey> ids);

        Task<IEnumerable<T>> FindBySqlQuery(string sql, object parameters);

        Task<QueryResult<T>> ExecuteQuery(string sql, object parameters);
    }
}
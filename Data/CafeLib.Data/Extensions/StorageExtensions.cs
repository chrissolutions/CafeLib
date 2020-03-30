using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Data.Persistence;
using CafeLib.Data.Sources;
using Dapper;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Extensions
{
    /// <summary>
    /// Database storage wrapper class.
    /// </summary>
    public static class StorageExtensions
    {
        /// <summary>
        /// Add an entity into storage.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entity">entity object</param>
        public static Task<T> Add<T>(this IStorage storage, T entity) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Add(entity);

        /// <summary>
        /// Add a collection of entities into storage.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entities">collection of IEntity</param>
        public static Task<bool> Add<T>(this IStorage storage, IEnumerable<T> entities) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Add(entities);

        /// <summary>
        /// Determines whether the repository has any entities.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <returns>
        ///     true: if the repository has entities.
        ///     false: if the repository is empty.
        /// </returns>
        public static Task<bool> Any<T>(this IStorage storage) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Any();

        /// <summary>
        /// Determines whether the collection contains any entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="storage">storage</param>
        /// <param name="id">key identifier</param>
        /// <returns>
        ///     true: if the repository has entities.
        ///     false: if the repository is empty.
        /// </returns>
        public static Task<bool> Any<T, TU>(this IStorage storage, TU id) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Any(id);

        /// <summary>
        /// Gets the number of entity items
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <returns>The count of items the T collection</returns>
        public static Task<int> Count<T>(this IStorage storage) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Count();

        /// <summary>
        /// Gets the number of entity items
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="exp">Linq expression</param>
        /// <param name="parameters">expression parameters</param>
        /// <returns>The count of items the T collection</returns>
        public static Task<int> Count<T>(this IStorage storage, Expression<Func<T, bool>> exp, object? parameters) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Count(exp, ToSqlParameters(parameters)?.ToArray<object>());

        /// <summary>
        /// Find all entities in a collection
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <returns>returns all the elements of a collection</returns>
        public static Task<IEnumerable<T>> FindAll<T>(this IStorage storage) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().FindAll();

        /// <summary>
        /// Find entities based on expression
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="exp">Linq expression</param>
        /// <param name="parameters">expression parameters</param>
        /// <returns>return collection of IEntity</returns>
        public static Task<IEnumerable<T>> Find<T>(this IStorage storage, Expression<Func<T, bool>> exp, object? parameters = null) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Find(exp, ToSqlParameters(parameters)?.ToArray<object>());

        /// <summary>
        /// Find entity by its key identifier.
        /// </summary>
        /// <typeparam name="T">Data transfer type</typeparam>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="key">key</param>
        /// <returns>data transfer object</returns>
        public static Task<T> FindByKey<T, TKey>(this IStorage storage, TKey key) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().FindByKey(key);

        /// <summary>
        /// Query using sql.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> FindBySql<T>(this IStorage storage, string sql, params object[] parameters) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().FindBySqlQuery(sql, parameters);

        /// <summary>
        /// Find one entity
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="exp">Linq expression</param>
        /// <param name="parameters">query parameters</param>
        /// <returns>returns an entity</returns>
        public static Task<T> FindOne<T>(this IStorage storage, Expression<Func<T, bool>> exp, object? parameters) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().FindOne(exp, ToSqlParameters(parameters)?.ToArray<object>());

        /// <summary>
        /// Removes an entity from a collection
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entity">entity to be deleted from the collection</param>
        public static Task<bool> Remove<T>(this IStorage storage, T entity) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Remove(entity);

        /// <summary>
        /// Removes an entity from a collection if the predicate is true.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="exp"></param>
        public static Task<int> Remove<T>(this IStorage storage, Expression<Func<T, bool>> exp) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Remove(exp);

        /// <summary>
        /// Remove entity from storage using entity id.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <typeparam name="TU">identifier type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="id">identifier</param>
        /// <returns>true if removed; false otherwise</returns>
        public static Task<bool> RemoveById<T, TU>(this IStorage storage, TU id) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().RemoveById(id);

        /// <summary>
        /// Remove entities via collection of identifiers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="storage">storage</param>
        /// <param name="idCollection"></param>
        /// <returns></returns>
        public static Task<bool> RemoveById<T, TU>(this IStorage storage, IEnumerable<TU> idCollection) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().RemoveById(idCollection);

        /// <summary>
        /// Save entity to database. 
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entity">entity object</param>
        public static Task<T> Save<T>(this IStorage storage, T entity) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Save(entity);


        /// <summary>
        /// Save entities to database. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="entities">collection of IEntity</param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static Task<int> Save<T>(this IStorage storage, IEnumerable<T> entities, params Expression<Func<T, object>>[]? expressions) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Save(entities, expressions);

        /// <summary>
        /// Update an entity into storage.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entity">IEntity</param>
        public static Task<bool> Update<T>(this IStorage storage, T entity) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Update(entity);

        /// <summary>
        /// Update a collection of entities into storage.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entities">collection of IEntity</param>
        public static Task<bool> Update<T>(this IStorage storage, IEnumerable<T> entities) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Update(entities);

        /// <summary>
        /// Execute sql command.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql">sql command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>execution result</returns>
        public static Task<int> Execute(this IStorage storage, string sql, object? parameters = null)
            => ((StorageBase)storage).GetConnection().ExecuteAsync(sql, ToSqlParameters(parameters)?.ToArray<object>());


        /// <summary>
        /// Execute sql query.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql">sql command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>query result</returns>
        public static Task<QueryResult<T>> ExecuteQuery<T>(this IStorage storage, string sql, object? parameters = null) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().ExecuteQuery(sql, ToSqlParameters(parameters)?.ToArray<object>());

        /// <summary>
        /// Execute sql save.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql">sql command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>save result</returns>
        public static Task<SaveResult<TU>> ExecuteSave<T, TU>(this IStorage storage, string sql, object? parameters = null) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().ExecuteSave<TU>(sql, ToSqlParameters(parameters)?.ToArray<object>());

        /// <summary>
        /// Convert an anonymous object of parameters to sql parameters.
        /// </summary>
        /// <param name="parameters">anonymous object of parameters</param>
        /// <returns>map of sql parameters></returns>
        private static IEnumerable<SqlParameter>? ToSqlParameters(object? parameters)
        {
            if (parameters == null) return null;
            var objectMap = new Dictionary<string, object>();
            parameters.GetType().GetProperties().ForEach(x => objectMap.Add(x.Name, x.GetValue(parameters)));
            return objectMap.Select(x => new SqlParameter(x.Key, x.Value));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
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
        /// Determine whether the entity has any entities.
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
        /// <param name="predicate">predicate condition</param>
        /// <returns>The count of items the T collection</returns>
        public static Task<int> Count<T>(this IStorage storage, Expression<Func<T, bool>> predicate) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Count(predicate);

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
        /// <param name="predicate">predicate condition</param>
        /// <returns>return collection of IEntity</returns>
        public static Task<IEnumerable<T>> Find<T>(this IStorage storage, Expression<Func<T, bool>> predicate) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Find(predicate);

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
        public static Task<IEnumerable<T>> FindBySql<T>(this IStorage storage, string sql, object parameters) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().FindBySqlQuery(sql, parameters);

        /// <summary>
        /// Find one entity
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="predicate">predicate condition</param>
        /// <returns>returns an entity</returns>
        public static Task<T> FindOne<T>(this IStorage storage, Expression<Func<T, bool>> predicate) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().FindOne(predicate);

        /// <summary>
        /// Remove an entity from storage.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entity">entity to be deleted from the collection</param>
        public static Task<bool> Remove<T>(this IStorage storage, T entity) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Remove(entity);

        /// <summary>
        /// Remove an entity from storage.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="entities">entities to be deleted from storage</param>
        /// <returns>number of entities removed from storage</returns>
        public static Task<int> Remove<T>(this IStorage storage, IEnumerable<T> entities) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Remove(entities);

        /// <summary>
        /// Removes an entity from a collection if the predicate is true.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="predicate">predicate condition</param>
        public static Task<int> Remove<T>(this IStorage storage, Expression<Func<T, bool>> predicate) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Remove(predicate);

        /// <summary>
        /// Remove entity from storage using entity primary key.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="key">primary key</param>
        /// <returns>true if removed; false otherwise</returns>
        public static Task<bool> RemoveByKey<T, TKey>(this IStorage storage, TKey key) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().RemoveByKey(key);

        /// <summary>
        /// Remove entities via collection of primary keys.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">primary key type</typeparam>
        /// <param name="storage">storage</param>
        /// <param name="keys">collection of primary keys</param>
        /// <returns></returns>
        public static Task<int> RemoveById<T, TKey>(this IStorage storage, IEnumerable<TKey> keys) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().RemoveByKey(keys);

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
        public static Task<int> Save<T>(this IStorage storage, IEnumerable<T> entities, params Expression<Func<T, object>>[] expressions) where T : class, IEntity
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
        public static Task<int> Update<T>(this IStorage storage, IEnumerable<T> entities) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().Update(entities);

        /// <summary>
        /// Execute sql command.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql">sql command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>execution result</returns>
        public static Task<int> Execute(this IStorage storage, string sql, object parameters = null)
            => ((StorageBase)storage).GetConnection().ExecuteAsync(sql, parameters);

        /// <summary>
        /// Execute sql query.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql">sql command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>query result</returns>
        public static Task<QueryResult<T>> ExecuteQuery<T>(this IStorage storage, string sql, object parameters = null) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().ExecuteQuery(sql, parameters);

        /// <summary>
        /// Execute sql save.
        /// </summary>
        /// <param name="storage">storage</param>
        /// <param name="sql">sql command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>save result</returns>
        public static Task<SaveResult<TU>> ExecuteSave<T, TU>(this IStorage storage, string sql, object parameters = null) where T : class, IEntity
            => ((StorageBase)storage).Repositories.Find<T>().ExecuteSave<TU>(sql, parameters);
    }
}
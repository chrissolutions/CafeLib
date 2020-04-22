using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Sources
{
    public interface ISqlCommandProvider
    {
        /// <summary>
        /// Delete entity from table.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if deleted, false if not found</returns>
        Task<bool> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Bulk delete of entity records.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<int> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Delete entity from table.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="predicate">predicate expression</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<int> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Delete by key
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="key"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<bool> DeleteByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="keys"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<int> DeleteByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, IEnumerable<TKey> keys, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Execute sql command.
        /// </summary>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default);

        /// <summary>
        /// Execute sql command to save an entry.
        /// </summary>
        /// <typeparam name="TKey">Entity key</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Upsert result</returns>
        Task<SaveResult<TKey>> ExecuteSaveAsync<TKey>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default);

        /// <summary>
        /// Execute sql command.
        /// </summary>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>scalar object</returns>
        Task<object> ExecuteScalarAsync(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default);

        /// <summary>
        /// Insert an entity record into the table.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Bulk insert entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>number of records inserted</returns>
        Task<int> InsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Query sql.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<QueryResult<TEntity>> QueryAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Query predicate.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="predicate">predicate expression</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<QueryResult<TEntity>> QueryAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Query all.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="token"></param>
        /// <returns>Query result</returns>
        Task<QueryResult<TEntity>> QueryAllAsync<TEntity>(IConnectionInfo connectionInfo, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="key"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<TEntity> QueryByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Query entities from a collection of keys.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="keys">keys</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<IEnumerable<TEntity>> QueryByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, IEnumerable<TKey> keys, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Query entity count.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<int> QueryCountAsync<TEntity>(IConnectionInfo connectionInfo, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Query count based on the entity key.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="key">entity key</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<int> QueryCountAsync<TEntity, TKey>(IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="predicate">predicate expression</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<int> QueryCountAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<TEntity> QueryOneAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="predicate">predicate expression</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        Task<TEntity> QueryOneAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Update an entity record.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Bulk update entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">Type to be updated</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Insert or update an entity record.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<TEntity> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity;

        /// <summary>
        /// Insert or update entities.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<int> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity;
    }
}

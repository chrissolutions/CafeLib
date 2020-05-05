using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Support;

namespace CafeLib.Data.Sources.Sqlite
{
    internal class SqliteCommandProvider : SingletonBase<SqliteCommandProvider>, ISqlCommandProvider
    {
        private static readonly SqlCommandProvider<SQLiteConnection> SqlCommandProvider = new SqlCommandProvider<SQLiteConnection>();

        /// <summary>
        /// SqliteCommandProvider constructor.
        /// </summary>
        public SqliteCommandProvider()
        {
            SqliteBootstrap.Initialize(new SqlDbSetting());
        }

        /// <summary>
        /// Delete entity from table.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if deleted, false if not found</returns>
        public async Task<bool> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.DeleteAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk delete of entity records.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.DeleteAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete entity from table.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="predicate">predicate expression</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.DeleteAsync(connectionInfo, predicate, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete by key
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="key"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<bool> DeleteByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.DeleteByKeyAsync<TEntity, TKey>(connectionInfo, key, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete by key.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="keys"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<int> DeleteByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, IEnumerable<TKey> keys, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.DeleteByKeyAsync<TEntity, TKey>(connectionInfo, keys, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute sql command.
        /// </summary>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            return await SqlCommandProvider.ExecuteAsync(connectionInfo, sql, parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute sql command to save an entry.
        /// </summary>
        /// <typeparam name="TKey">Entity key</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Upsert result</returns>
        public async Task<SaveResult<TKey>> ExecuteSaveAsync<TKey>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            return await SqlCommandProvider.ExecuteSaveAsync<TKey>(connectionInfo, sql, parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            return await SqlCommandProvider.ExecuteScalarAsync(connectionInfo, sql, parameters, token);
        }

        /// <summary>
        /// Sql insert command
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<TEntity> InsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.InsertAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> InsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.InsertAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Query raw sql.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<QueryResult<TEntity>> QueryAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryAsync<TEntity>(connectionInfo, sql, parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<QueryResult<TEntity>> QueryAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryAsync(connectionInfo, predicate, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Query all.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="token"></param>
        /// <returns>Query result</returns>
        public async Task<QueryResult<TEntity>> QueryAllAsync<TEntity>(IConnectionInfo connectionInfo, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryAllAsync<TEntity>(connectionInfo, token).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TEntity> QueryByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryByKeyAsync<TEntity, TKey>(connectionInfo, key, token).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="keys"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> QueryByKeyAsync<TEntity, TKey>(IConnectionInfo connectionInfo, IEnumerable<TKey> keys, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryByKeyAsync<TEntity, TKey>(connectionInfo, keys, token).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<int> QueryCountAsync<TEntity>(IConnectionInfo connectionInfo, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryCountAsync<TEntity>(connectionInfo, token);
        }

        /// <summary>
        /// Query count based on the entity key.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="key">entity key</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<int> QueryCountAsync<TEntity, TKey>(IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryCountAsync<TEntity, TKey>(connectionInfo, key, token).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<int> QueryCountAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryCountAsync(connectionInfo, predicate, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TEntity> QueryOneAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryOneAsync<TEntity>(connectionInfo, sql, parameters, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TEntity> QueryOneAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryOneAsync(connectionInfo, predicate, token);
        }

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.UpdateAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="TEntity">Type to be updated</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>number of records updated.</returns>
        public async Task<int> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.UpdateAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert or update entities into table.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        public async Task<TEntity> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.UpsertAsync(connectionInfo, data, expressions, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts entities into table.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        public async Task<int> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, Expression<Func<TEntity, object>>[] expressions, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.UpsertAsync(connectionInfo, data, expressions, token);
        }
    }
}

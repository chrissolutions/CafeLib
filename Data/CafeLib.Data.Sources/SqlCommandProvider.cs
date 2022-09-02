using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Sources.Extensions;
using RepoDb;

namespace CafeLib.Data.Sources
{
    public class SqlCommandProvider<T> : ISqlCommandProvider where T : IDbConnection, IAsyncDisposable
    {
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
            if (data == null) throw new ArgumentNullException(nameof(data));
            var results = 0;
            await using var connection = connectionInfo.GetConnection<T>();
            await connection.EnsureOpenAsync(cancellationToken: token).ConfigureAwait(false);
            using var transaction = connection.BeginTransaction();

            try
            {
                results = await connection.DeleteAsync(data, cancellationToken: token).ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception)
                {
                    throw; 
                }
            }

            return results > 0;
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
            if (data == null) throw new ArgumentNullException(nameof(data));
            await using var connection = connectionInfo.GetConnection<T>();
            using var transaction = connection.BeginTransaction();

            try
            {
                var deleted = await connection.DeleteAllAsync(data, transaction: transaction, cancellationToken: token).ConfigureAwait(false);
                transaction.Commit();
                return deleted;
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
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
            await using var connection = connectionInfo.GetConnection<T>();
            return await connection.DeleteAsync(predicate, cancellationToken: token).ConfigureAwait(false);
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
            return await DeleteByKeyAsync<TEntity, TKey>(connectionInfo, new[]{key}, token).ConfigureAwait(false) > 0;
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
            await using var connection = connectionInfo.GetConnection<T>();
            return await connection.DeleteAllAsync<TEntity>(keys.Cast<object>(), cancellationToken: token).ConfigureAwait(false);
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
            await using var connection = connectionInfo.GetConnection<T>();
            return await connection.ExecuteNonQueryAsync(sql, parameters, cancellationToken: token).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute sql command to save an entry.
        /// </summary>
        /// <typeparam name="TKey">Entity key</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Save result</returns>
        public async Task<SaveResult<TKey>> ExecuteSaveAsync<TKey>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            await using var connection = connectionInfo.GetConnection<T>();
            var result = await connection.ExecuteScalarAsync(sql, parameters, cancellationToken: token).ConfigureAwait(false);
            return result != null ? new SaveResult<TKey>((TKey)result, true) : new SaveResult<TKey>((TKey)DefaultSqlId<TKey>());
        }

        /// <summary>
        /// Execute sql scalar command.
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            await using var connection = connectionInfo.GetConnection<T>();
            return await connection.ExecuteScalarAsync(sql, parameters, cancellationToken: token).ConfigureAwait(false);
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
            if (data == null) throw new ArgumentNullException(nameof(data));
            await using var connection = connectionInfo.GetConnection<T>();
            var id = await connection.InsertAsync(data, cancellationToken: token).ConfigureAwait(false);
            var propertyInfo = PrimaryCache.Get<TEntity>().PropertyInfo;
            propertyInfo.SetValue(data, Convert.ChangeType(id, propertyInfo.PropertyType));
            return data;
        }

        /// <summary>
        /// Bulk insert entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>number of records inserted</returns>
        public async Task<int> InsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            await using var connection = connectionInfo.GetConnection<T>();
            var inserted = await connection.InsertAllAsync(data, cancellationToken: token).ConfigureAwait(false);
            return inserted;
        }

        /// <summary>
        /// Query sql.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<QueryResult<TEntity>> QueryAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            using var result = await connection.ExecuteQueryMultipleAsync(sql, parameters, cancellationToken: token).ConfigureAwait(false);
            var entities = result.Extract<TEntity>()?.ToArray();
            var scalar = result.Scalar<int>();
            var totalCount = scalar != 0 ? scalar : entities?.Length ?? -1;
            return new QueryResult<TEntity> { Records = entities, TotalCount = totalCount };
        }

        /// <summary>
        /// Query predicate.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="predicate">predicate expression</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<QueryResult<TEntity>> QueryAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token) where TEntity : class,  IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            var result = (await connection.QueryAsync(predicate, cancellationToken: token).ConfigureAwait(false)).ToArray();
            return new QueryResult<TEntity> { Records = result, TotalCount = result.Length };
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
            await using var connection = connectionInfo.GetConnection<T>();
            var result = (await connection.QueryAllAsync<TEntity>(cancellationToken: token).ConfigureAwait(false)).ToArray();
            return new QueryResult<TEntity> { Records = result, TotalCount = result.Length };
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
            await using var connection = connectionInfo.GetConnection<T>();
            var queryField = new QueryField(PrimaryCache.Get<TEntity>().AsField(), key);
            return (await connection.QueryAsync<TEntity>(queryField, cancellationToken: token).ConfigureAwait(false)).FirstOrDefault();
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
            await using var connection = connectionInfo.GetConnection<T>();
            var queryField = new QueryField(PrimaryCache.Get<TEntity>().AsField(), keys);
            return await connection.QueryAsync<TEntity>(queryField, cancellationToken: token).ConfigureAwait(false);
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
            await using var connection = connectionInfo.GetConnection<T>();
            return (int)await connection.CountAllAsync<TEntity>(cancellationToken: token).ConfigureAwait(false);
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
            var keyColumnName = PrimaryCache.Get<TEntity>().GetMappedName();
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var result = await ExecuteAsync(connectionInfo, $@"SELECT TOP 1 {keyColumnName} FROM {tableName} WHERE {keyColumnName} = {CheckSqlKey(key)}", null, token).ConfigureAwait(false);
            return result;
        }

        public async Task<int> QueryCountAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            return (int)await connection.CountAsync(predicate, cancellationToken: token).ConfigureAwait(false);
        }

        public async Task<TEntity> QueryOneAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            return (await connection.ExecuteQueryAsync<TEntity>(sql, parameters, cancellationToken: token).ConfigureAwait(false)).FirstOrDefault();
        }

        public async Task<TEntity> QueryOneAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            return (await connection.QueryAsync(predicate, cancellationToken: token).ConfigureAwait(false)).FirstOrDefault();
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
            await using var connection = connectionInfo.GetConnection<T>();
            var updated = await connection.UpdateAsync(data, cancellationToken: token).ConfigureAwait(false);
            return updated > 0;
        }

        /// <summary>
        /// Bulk update entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">Type to be updated</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<int> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            return await connection.UpdateAllAsync(data, cancellationToken: token).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert or update an entity record.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<TEntity> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            await using var connection = connectionInfo.GetConnection<T>();
            var id = await connection.MergeAsync(data, cancellationToken: token).ConfigureAwait(false);
            var propertyInfo = PrimaryCache.Get<TEntity>().PropertyInfo;
            propertyInfo.SetValue(data, Convert.ChangeType(id, propertyInfo.PropertyType));
            return data;
        }

        /// <summary>
        /// Bulk Insert or update of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, Expression<Func<TEntity, object>>[] expressions, CancellationToken token = default) where TEntity : class, IEntity
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var results = 0;
            await using var connection = connectionInfo.GetConnection<T>();
            await connection.EnsureOpenAsync(cancellationToken: token).ConfigureAwait(false);
            using var transaction = connection.BeginTransaction();

            try
            {
                results = await connection.MergeAllAsync(data, transaction: transaction, cancellationToken: token).ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return results;
        }

        #region Helpers

        /// <summary>
        /// Check Sql key type for requiring quotation.
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="id"></param>
        /// <returns>return quoted id for certain types</returns>
        private static string CheckSqlKey<TKey>(TKey id)
        {
            switch (id?.GetType().Name)
            {
                case "String":
                case "Guid":
                    return $"'{id}'";

                case null:
                    return @"NULL";

                default:
                    return $"{id}";
            }
        }

        /// <summary>
        /// Check Sql key type for requiring quotation.
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <returns>return quoted id for certain types</returns>
        private static object DefaultSqlId<TKey>()
        {
            switch (typeof(TKey).Name)
            {
                case "String":
                    return string.Empty;

                case "Guid":
                    return Guid.Empty;

                default:
                    return 0;
            }
        }

        #endregion
    }
}

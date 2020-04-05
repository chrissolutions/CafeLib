using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Sources.Extensions;
using Dapper;

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
        public async Task<bool> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : IEntity
        {
            if (data == null)
                throw new ArgumentException("Cannot Delete null Object", nameof(data));

            var deleted = await connectionInfo.ExecuteAsync(SqlCommandFormatter.FormatDeleteStatement<TEntity>(connectionInfo.Domain), data, token).ConfigureAwait(false);
            return deleted > 0;
        }

        /// <summary>
        /// Bulk delete of entity records.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var entity in data)
                {
                    var command = new CommandDefinition(SqlCommandFormatter.FormatDeleteStatement<TEntity>(connectionInfo.Domain), entity, transaction);
                    await connection.ExecuteAsync(command).ConfigureAwait(false);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }
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
            return await connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<QueryResult<TEntity>> ExecuteQueryAsync<TEntity>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token) where TEntity : IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            var command = new CommandDefinition(sql, parameters);
            using var reader = await connection.ExecuteReaderAsync(command).ConfigureAwait(false);

            var totalCount = -1;
            var results = new List<TEntity>();
            var model = Activator.CreateInstance<TEntity>();

            while (reader.Read())
            {
                foreach (var prop in model.GetType().GetProperties())
                {
                    var attr = prop.GetCustomAttribute(typeof(ColumnAttribute));
                    var name = attr != null ? ((ColumnAttribute)attr).Name : prop.Name;
                    var val = reader[name];
                    prop.SetValue(model, val == DBNull.Value ? null : val);
                }
                results.Add(model);
                model = Activator.CreateInstance<TEntity>();
            }

            if (reader.NextResult())
            {
                reader.Read();
                totalCount = reader.GetInt32(0);
            }

            return new QueryResult<TEntity> { Records = results.ToArray(), TotalCount = totalCount };
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
        public async Task<SaveResult<TKey>> ExecuteSave<TKey>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            await using var connection = connectionInfo.GetConnection<T>();
            var command = new CommandDefinition(sql, parameters);
            var id = (TKey)DefaultSqlId<TKey>();

            var result = await connection.ExecuteScalarAsync(command).ConfigureAwait(false);
            return result != null ? new SaveResult<TKey>((TKey)result, true) : new SaveResult<TKey>(id);
        }

        /// <summary>
        /// Sql insert command
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public Task<TEntity> InsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : IEntity
        {
            throw new InvalidOperationException($"{GetType().Name} does not implement {nameof(InsertAsync)}.");
        }

        public Task<int> InsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : IEntity
        {
            throw new InvalidOperationException($"{GetType().Name} does not implement {nameof(InsertAsync)}.");
        }

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            var updated = await connection.ExecuteAsync(SqlCommandFormatter.FormatUpdateStatement<TEntity>(connectionInfo.Domain), data).ConfigureAwait(false);
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
        public async Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : IEntity
        {
            await using var connection = connectionInfo.GetConnection<T>();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var entity in data)
                {
                    var command = new CommandDefinition(SqlCommandFormatter.FormatUpdateStatement<TEntity>(connectionInfo.Domain), entity, transaction);
                    await connection.ExecuteAsync(command).ConfigureAwait(false);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }
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
        public async Task<TEntity> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : IEntity
        {
            if (data.IsKeyGenerated() && data.KeyValue() == data.KeyDefaultValue())
            {
                return await connectionInfo.InsertAsync(data, token);
            }

            await connectionInfo.UpdateAsync(data, token);
            return data;
        }

        /// <summary>
        /// Insert or update entities.
        /// </summary>
        /// <typeparam name="TEntity">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public Task<int> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, Expression<Func<TEntity, object>>[] expressions, CancellationToken token = default) where TEntity : IEntity
        {
            throw new InvalidOperationException($"{GetType().Name} does not implement {nameof(UpdateAsync)}.");
        }

        #region Helpers

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

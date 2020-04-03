using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Data.Sources.Extensions;
using Dapper;

namespace CafeLib.Data.Sources
{
    public class SqlCommandProvider<T> : ISqlCommandProvider where T : IDbConnection, IAsyncDisposable
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly KeyValuePair<string, object>[] EmptyParameters = new KeyValuePair<string, object>[0];

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
                    await connection.ExecuteAsync(command);
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
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var commandParameters = (DbParameterCollection)command.Parameters;
            commandParameters.AddRange(parameters?.ToObjectMap().ToArray() ?? EmptyParameters);

            connection.Open();
            using var reader = command.ExecuteReader();
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

            return await Task.FromResult(new QueryResult<TEntity> { Records = results.ToArray(), TotalCount = totalCount });
        }

        /// <summary>
        /// Execute sql insert or update command.
        /// </summary>
        /// <typeparam name="TKey">Entity key</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Upsert result</returns>
        public async Task<SaveResult<TKey>> ExecuteUpsert<TKey>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            await using var connection = connectionInfo.GetConnection<T>();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var commandParameters = (DbParameterCollection) command.Parameters;
            commandParameters.AddRange( parameters?.ToObjectMap().ToArray() ?? EmptyParameters);

            var inserted = false;
            var id = (TKey)DefaultSqlId<TKey>();

            connection.Open();
            var result = await Task.FromResult(command.ExecuteScalar());
            if (result != null)
            {
                id = (TKey)result;
                inserted = true;
            }

            return new SaveResult<TKey>(id, inserted);
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
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : IEntity
        {
            throw new NotImplementedException();
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
            using var cmd = connection.CreateCommand();

            var tableName = connectionInfo.Domain.TableCache.TableName<TEntity>();
            var keyProperties = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<TEntity>();
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var allProperties = connectionInfo.Domain.PropertyCache.TypePropertiesCache<TEntity>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<TEntity>();
            var computedProperties = connectionInfo.Domain.PropertyCache.ComputedPropertiesCache<TEntity>();
            var nonKeyProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            var sb = new StringBuilder();
            sb.Append($"UPDATE {tableName} SET ");

            nonKeyProps.ForEach(x =>
            {
                sb.Append($"{columns[x.Name]} = @{x.Name}");
                sb.Append(", ");
            });
            sb.Remove(sb.Length - ", ".Length, ", ".Length);

            sb.Append(" WHERE ");

            for (var i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties[i];
                sb.Append($"{property.Name} = @{property.Name}");
                if (i < keyProperties.Count - 1)
                    sb.Append(" AND ");
            }

            var updated = await connection.ExecuteAsync(sb.ToString(), data).ConfigureAwait(false);
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
        public Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : IEntity
        {
            throw new NotImplementedException();
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
        public Task<int> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions, CancellationToken token = default) where TEntity : IEntity
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

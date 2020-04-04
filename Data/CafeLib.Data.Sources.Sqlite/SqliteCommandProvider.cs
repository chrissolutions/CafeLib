﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
using CafeLib.Data.Sources.Extensions;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CafeLib.Data.Sources.Sqlite
{
    internal class SqliteCommandProvider : SingletonBase<SqliteCommandProvider>, ISqlCommandProvider
    {
        private static readonly SqlCommandProvider<SqliteConnection> SqlCommandProvider = new SqlCommandProvider<SqliteConnection>();

        /// <summary>
        /// Delete entity from table.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if deleted, false if not found</returns>
        public async Task<bool> DeleteAsync<T>(IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            return await SqlCommandProvider.DeleteAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk delete of entity records.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            return await SqlCommandProvider.DeleteAsync(connectionInfo, data, token).ConfigureAwait(false);
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
        /// Execute sql query
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Query result</returns>
        public async Task<QueryResult<T>> ExecuteQueryAsync<T>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where T : IEntity
        {
            return await SqlCommandProvider.ExecuteQueryAsync<T>(connectionInfo, sql, parameters, token).ConfigureAwait(false);
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
        public async Task<SaveResult<TKey>> ExecuteSave<TKey>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            return await SqlCommandProvider.ExecuteSave<TKey>(connectionInfo, sql, parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sql insert command
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<T> InsertAsync<T>(IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            var sqlFormat = SqlCommandFormatter.FormatInsertStatement<T>(connectionInfo.Domain);
            var tableName = connectionInfo.Domain.TableCache.TableName<T>();
            var keyName = connectionInfo.Domain.PropertyCache.PrimaryKeyName<T>();
            var sql = sqlFormat.Replace("-- Placeholder02 --", $"select * from {tableName} where {keyName} = last_insert_rowid()");
            await using var connection = connectionInfo.GetConnection<SqliteConnection>();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert entities asynchronously.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            await using var connection = connectionInfo.GetConnection<SqliteConnection>();
            await using var transaction = connection.BeginTransaction();
            var count = 0;

            try
            {
                foreach (var entity in data)
                {
                    var sqlFormat = SqlCommandFormatter.FormatInsertStatement<T>(connectionInfo.Domain);
                    var sql = sqlFormat.Replace("-- Placeholder02 --", $"select last_insert_rowid()");

                    var command = new CommandDefinition(sql, entity, transaction);
                    var key = connection.QuerySingleOrDefaultAsync(command);

                    var property = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<T>().First();
                    property.SetValue(entity, key);
                    ++count;
                }

                transaction.Commit();
                return count;
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
        /// Sql update command
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<bool> UpdateAsync<T>(IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            return await SqlCommandProvider.UpdateAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public Task<bool> UpdateAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert or update entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        public Task<int> UpsertAsync<T>(IConnectionInfo connectionInfo, T data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Collection of entity records</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        public Task<int> UpsertAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private static string GetColumnsStringSqlServer(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<string, string> columnNames, string tablePrefix = null)
        {
            if (tablePrefix == "target.")
            {
                return string.Join(", ", properties.Select(property => $"{tablePrefix}[{columnNames[property.Name]}] as [{property.Name}]"));
            }

            return string.Join(", ", properties.Select(property => $"{tablePrefix}[{columnNames[property.Name]}]"));
        }

        private static DataTable ToDataTable<T>(IEnumerable<T> data, IReadOnlyList<PropertyInfo> properties)
        {
            var typeCasts = new Type[properties.Count];
            for (var i = 0; i < properties.Count; i++)
            {
                if (properties[i].PropertyType.IsEnum)
                {
                    typeCasts[i] = Enum.GetUnderlyingType(properties[i].PropertyType);
                }
                else
                {
                    typeCasts[i] = null;
                }
            }

            var dataTable = new DataTable();
            for (var i = 0; i < properties.Count; i++)
            {
                // Nullable types are not supported.
                var propertyNonNullType = Nullable.GetUnderlyingType(properties[i].PropertyType) ?? properties[i].PropertyType;
                dataTable.Columns.Add(properties[i].Name, typeCasts[i] ?? propertyNonNullType);
            }

            foreach (var item in data)
            {
                var values = new object[properties.Count];
                for (var i = 0; i < properties.Count; i++)
                {
                    var value = properties[i].GetValue(item, null);
                    values[i] = typeCasts[i] == null ? value : Convert.ChangeType(value, typeCasts[i]);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private static string FormatTableName(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                return table;
            }

            var parts = table.Split('.');

            if (parts.Length == 1)
            {
                return $"[{table}]";
            }

            return $"[{parts[0]}].[{parts[1]}]";
        }

        private static IEnumerable<string> FormatColumnNames(string columnsString, string prefix)
        {
            return columnsString.Split(',').Select(x => $"[{prefix}].{x.Trim()}");
        }

        private static IEnumerable<string> FormatUpdateList(string columnsString, string source = "src", string target = "tgt")
        {
            return columnsString.Split(',').Select(x => $"[{target}].{x.Trim()} = [{source}].{x.Trim()}");
        }

        private static IEnumerable<string> FormatMergeOnMatchList<T>(Domain domain, PropertyExpressionList<T> expressionList, string source = "src", string target = "tgt") where T : IEntity
        {
            const string template = "[{target}].{targetKey}=[{source}].{sourceKey}";
            var results = new List<string>();

            if (expressionList != null && expressionList.Any())
            {
                var columnNames = expressionList.GetColumnNames();
                columnNames.ForEach(x => results.Add(template.Render(new { target, targetKey = x, source, sourceKey = x })));
            }
            else
            {
                var keyName = domain.PropertyCache.KeyPropertiesCache<T>().First().Name;
                results.Add(template.Render(new { target, targetKey = keyName, source, sourceKey = keyName }));
            }

            return results;
        }

        #endregion
    }
}

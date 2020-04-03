using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
using CafeLib.Data.Sources.Extensions;
using Dapper;

namespace CafeLib.Data.Sources.SqlServer
{
    internal class SqlServerCommandProvider : SingletonBase<SqlServerCommandProvider>, ISqlCommandProvider
    {
        private static readonly SqlCommandProvider<SqlConnection> SqlCommandProvider = new SqlCommandProvider<SqlConnection>();

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
            return await SqlCommandProvider.ExecuteUpsert<TKey>(connectionInfo, sql, parameters, token).ConfigureAwait(false);
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
            var tableName = connectionInfo.Domain.TableCache.TableName<T>();
            var allProperties = connectionInfo.Domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = connectionInfo.Domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = SqlCommandFormatter.FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);

            var sbParameterList = new StringBuilder(null);
            for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++)
            {
                var property = allPropertiesExceptKeyAndComputed[i];
                sbParameterList.Append($"@{property.Name}");
                if (i < allPropertiesExceptKeyAndComputed.Count - 1)
                    sbParameterList.Append(", ");
            }

            var sql = $@"
            INSERT INTO { FormatTableName(tableName)} ({ allPropertiesExceptKeyAndComputedString}) 
            OUTPUT INSERTED.*
            VALUES({sbParameterList})";

            await using var connection = connectionInfo.GetConnection<SqlConnection>();
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
            var tableName = connectionInfo.Domain.TableCache.TableName<T>();
            var allProperties = connectionInfo.Domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = connectionInfo.Domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesString = SqlCommandFormatter.FormatColumnsToString(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = SqlCommandFormatter.FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            var properties = keyProperties.First().PropertyType == typeof(Guid) ? allProperties : allPropertiesExceptKeyAndComputed;
            var propertiesString = keyProperties.First().PropertyType == typeof(Guid) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;

            // Open connection.
            await using var connection = connectionInfo.GetConnection<SqlConnection>();
            connection.Open();

            // Create temporary table to cache resultant bulk copy.
            await connection.ExecuteAsync($@"SELECT TOP 0 {propertiesString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);").ConfigureAwait(false);

            var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null);

            try
            {
                // Perform bulk copy
                using (var bulkCopy = sqlBulkCopy) //new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = 30;
                    bulkCopy.BatchSize = 0;
                    bulkCopy.DestinationTableName = tempToBeInserted;

                    // ReSharper disable once AccessToDisposedClosure
                    properties.ForEach((x, i) => bulkCopy.ColumnMappings.Add(x.Name, columns[x.Name]));
                    await using var table = ToDataTable(data, properties).CreateDataReader();
                    await bulkCopy.WriteToServerAsync(table, token).ConfigureAwait(false);
                }

                // Insert from temporary table to actual table.
                var sql = $@"INSERT INTO {FormatTableName(tableName)}({propertiesString}) SELECT {propertiesString} FROM {tempToBeInserted}";
                var result = await connection.ExecuteAsync(sql).ConfigureAwait(false); 

                await connection.ExecuteAsync($@"DROP TABLE {tempToBeInserted};").ConfigureAwait(false);
                connection.Close();
                return result;
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains(@"Received an invalid column length from the bcp client for colid"))
                {
                    const string pattern = @"\d+";
                    var match = Regex.Match(ex.Message, pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    var fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi?.GetValue(sqlBulkCopy);
                    var items = (object[])sortedColumns?.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(sortedColumns);

                    var itemData = items?[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = items != null ? itemData?.GetValue(items[index]) : null;

                    var column = metadata?.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(metadata);
                    var length = metadata?.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(metadata);
                    throw new DataException($"Column: {column} contains data with a length greater than: {length}");
                }

                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Update an entity record.
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
        /// Bulk update entities asynchronously.
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
        /// Insert or update entities into table.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        public Task<int> UpsertAsync<T>(IConnectionInfo connectionInfo, T data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert or update an entity record.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> UpsertAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity
        {
            var tableName = connectionInfo.Domain.TableCache.TableName<T>();
            var allProperties = connectionInfo.Domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = connectionInfo.Domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesString = SqlCommandFormatter.FormatColumnsToString(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = SqlCommandFormatter.FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            var propertiesString = keyProperties.First().PropertyType == typeof(Guid) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;
            var expressionList = new PropertyExpressionList<T>(connectionInfo.Domain, expressions);

            // Open connection.
            await using var connection = connectionInfo.GetConnection<SqlConnection>();
            connection.Open();

            // Create temporary table to cache resultant bulk copy.
            await connection.ExecuteAsync($@"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);").ConfigureAwait(false);
            await connection.ExecuteAsync($@"ALTER TABLE {tempToBeInserted} ADD {keyProperties.First().Name} {PropertyCache.GetSqlType(keyProperties.First().PropertyType)}").ConfigureAwait(false);

            try
            {
                // Perform bulk copy
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null))
                {
                    bulkCopy.BulkCopyTimeout = 30;
                    bulkCopy.BatchSize = 0;
                    bulkCopy.DestinationTableName = tempToBeInserted;

                    // ReSharper disable once AccessToDisposedClosure
                    allProperties.ForEach((x, i) => bulkCopy.ColumnMappings.Add(x.Name, columns[x.Name]));
                    await using var table = ToDataTable(data, allProperties).CreateDataReader();
                    await bulkCopy.WriteToServerAsync(table, token).ConfigureAwait(false);
                }

                //Now use the merge command to upsert from the temp table to the production table
                var sql = $@"MERGE INTO {FormatTableName(tableName)} as tgt  
                                    USING {tempToBeInserted} as src
                                    ON 
                                        {string.Join(" AND ", FormatMergeOnMatchList(connectionInfo.Domain, expressionList))}
                                    WHEN MATCHED THEN
                                        UPDATE SET
                                            {string.Join(", ", FormatUpdateList(propertiesString))}
                                    WHEN NOT MATCHED THEN
                                        INSERT 
                                            ({propertiesString}) 
                                        VALUES
                                            ({string.Join(", ", FormatColumnNames(propertiesString, "src"))});";


                // Merge from temporary table to actual table.
                var result = await connection.ExecuteAsync(sql).ConfigureAwait(false);

                await connection.ExecuteAsync($@"DROP TABLE {tempToBeInserted};").ConfigureAwait(false);
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #region Helpers

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

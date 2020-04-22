using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
using CafeLib.Data.Sources.Extensions;
using Dapper;
using RepoDb;

namespace CafeLib.Data.Sources.SqlServer
{
    internal class SqlServerCommandProvider : SingletonBase<SqlServerCommandProvider>, ISqlCommandProvider
    {
        private static readonly SqlCommandProvider<SqlConnection> SqlCommandProvider = new SqlCommandProvider<SqlConnection>();

        /// <summary>
        /// SqlServerCommandProvider constructor.
        /// </summary>
        public SqlServerCommandProvider()
        {
            SqlServerBootstrap.Initialize();
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
            return await SqlCommandProvider.ExecuteScalarAsync(connectionInfo, sql, parameters, token).ConfigureAwait(false);
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
            var sql = SqlCommandFormatter.FormatInsertStatement<TEntity>(connectionInfo.Domain).Replace("VALUES", "OUTPUT INSERTED.* VALUES");
            await using var connection = connectionInfo.GetConnection<SqlConnection>();
            return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, data).ConfigureAwait(false);
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
            var tableName = connectionInfo.Domain.TableCache.TableName<TEntity>();
            var allProperties = connectionInfo.Domain.PropertyCache.TypePropertiesCache<TEntity>();
            var keyProperties = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<TEntity>();
            var computedProperties = connectionInfo.Domain.PropertyCache.ComputedPropertiesCache<TEntity>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<TEntity>();

            var allPropertiesString = SqlCommandFormatter.FormatColumnsToString(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = SqlCommandFormatter.FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            var properties = keyProperties.First().PropertyType.IsPrimitive ? allPropertiesExceptKeyAndComputed : allProperties;
            var propertiesString = ReferenceEquals(properties, allProperties) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;

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
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<int> QueryCountAsync<TEntity>(IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.QueryCountAsync(connectionInfo, predicate, token).ConfigureAwait(false);
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
            return await SqlCommandProvider.QueryOneAsync<TEntity>(connectionInfo, sql, parameters, token).ConfigureAwait(false);
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
            return await SqlCommandProvider.QueryOneAsync(connectionInfo, predicate, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update an entity record.
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
        /// Bulk update entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<bool> UpdateAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity
        {
            return await SqlCommandProvider.UpdateAsync(connectionInfo, data, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert or update entities into table.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="connectionInfo">Connection info</param>
        /// <param name="data">Entity record</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        public async Task<TEntity> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity
        {
            
            var tableName = connectionInfo.Domain.TableCache.TableName<TEntity>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<TEntity>();
            var primaryKey = connectionInfo.Domain.PropertyCache.PrimaryKey<TEntity>();

            var updateStatement = SqlCommandFormatter.FormatUpdateStatement(connectionInfo.Domain, expressions).Replace("WHERE", "OUTPUT INSERTED.* WHERE");
            var inputStatement = SqlCommandFormatter.FormatInsertStatement<TEntity>(connectionInfo.Domain).Replace("VALUES", "OUTPUT INSERTED.* VALUES");

            var sql = $@"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                        BEGIN TRAN
                         IF EXISTS (SELECT 1 FROM {tableName} WHERE {columns[primaryKey.Name]} = @{columns[primaryKey.Name]})
                            {updateStatement}
                         ELSE	
                            {inputStatement}
                        COMMIT";

            await using var connection = connectionInfo.GetConnection<SqlConnection>();
            return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, data).ConfigureAwait(false);
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
        public async Task<int> UpsertAsync<TEntity>(IConnectionInfo connectionInfo, IEnumerable<TEntity> data, Expression<Func<TEntity, object>>[] expressions, CancellationToken token = default) where TEntity : class, IEntity
        {
            var tableName = connectionInfo.Domain.TableCache.TableName<TEntity>();
            var allProperties = connectionInfo.Domain.PropertyCache.TypePropertiesCache<TEntity>();
            var keyProperties = connectionInfo.Domain.PropertyCache.KeyPropertiesCache<TEntity>();
            var computedProperties = connectionInfo.Domain.PropertyCache.ComputedPropertiesCache<TEntity>();
            var columns = connectionInfo.Domain.PropertyCache.GetColumnNamesCache<TEntity>();
            var primaryKey = connectionInfo.Domain.PropertyCache.PrimaryKey<TEntity>();

            var allPropertiesString = SqlCommandFormatter.FormatColumnsToString(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = SqlCommandFormatter.FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            var propertiesString = primaryKey.PropertyType.IsPrimitive ? allPropertiesExceptKeyAndComputedString : allPropertiesString;
            var expressionList = new PropertyExpressionList<TEntity>(connectionInfo.Domain, expressions);
            // Open connection.
            await using var connection = connectionInfo.GetConnection<SqlConnection>();
            connection.Open();

            // Create temporary table to cache resultant bulk copy.
            await connection.ExecuteAsync($@"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);").ConfigureAwait(false);
            await connection.ExecuteAsync($@"ALTER TABLE {tempToBeInserted} ADD {keyProperties.First().Name} {GetSqlType(keyProperties.First().PropertyType)}").ConfigureAwait(false);

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
                                        {string.Join(", ", FormatUpdateList(allPropertiesExceptKeyAndComputedString))}
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

            if (expressionList.Any())
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

        public static string GetSqlType(Type type)
        {
            return type switch
            {
                { } when type == typeof(int) => "INT",

                { } when type == typeof(long) => "BIGINT",

                { } when type == typeof(Guid) => "UNIQUEIDENTIFER",

                _ => string.Empty,
            };
        }

        #endregion
    }
}

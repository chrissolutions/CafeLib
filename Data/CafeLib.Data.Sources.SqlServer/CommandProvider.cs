using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
using Dapper;

namespace CafeLib.Data.Sources.SqlServer
{
    internal class CommandProvider : SingletonBase<CommandProvider>, ISqlCommandProcessor
    {
        public async Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            if (data == null)
                throw new ArgumentException("Cannot Delete null Object", nameof(data));

            var type = typeof(T);

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                var typeInfo = type.GetTypeInfo();
                var implementsGenericIEnumerableOrIsGenericIEnumerable =
                    typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                    typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                if (implementsGenericIEnumerableOrIsGenericIEnumerable)
                {
                    type = type.GetGenericArguments()[0];
                }
            }

            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>().ToList();  //added ToList() due to issue #418, must work on a list copy
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var name = domain.TableCache.TableName(type);

            var sb = new StringBuilder();
            sb.Append($"DELETE FROM {name} WHERE ");

            for (var i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties[i];
                sb.Append($"{property.Name} = @{property.Name}");
                if (i < keyProperties.Count - 1)
                {
                    sb.Append(" AND ");
                }
            }

            var deleted = await connection.ExecuteAsync(sb.ToString(), data).ConfigureAwait(false);
            return deleted > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="domain"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public async Task<QueryResult<T>> ExecuteQueryAsync<T>(IDbConnection connection, string sql, object parameters) where T : class, IEntity
        {
            var conn = (SqlConnection)connection;
            var command = conn.CreateCommand();
            command.CommandText = sql;
            var args = typeof(SqlParameter).ToObjectMap(parameters);
            if (args?.Any() ?? false)
            {
                command.Parameters.AddRange(args.ToArray());
            }

            conn.Open();
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var totalCount = -1;
            var results = new List<T>();

            var model = Activator.CreateInstance<T>();
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
                model = Activator.CreateInstance<T>();
            }

            if (reader.NextResult())
            {
                reader.Read();
                totalCount = reader.GetInt32(0);
            }
            return new QueryResult<T> { Records = results.ToArray(), TotalCount = totalCount };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<SaveResult<TKey>> ExecuteUpsert<TKey>(IDbConnection connection, string sql, object parameters)
        {
            var conn = (SqlConnection)connection;
            conn.Open();
            await using var command = conn.CreateCommand();
            command.CommandText = sql;
            //if (parameters?.Any() ?? false)
            //{
            //    command.Parameters.AddRange(parameters.ToArray());
            //}

            var inserted = false;
            var id = (TKey)DefaultSqlId<TKey>();

            var result = await Task.FromResult(command.ExecuteScalar());
            if (result != null)
            {
                id = (TKey)result;
                inserted = true;
            }

            return new SaveResult<TKey>(id, inserted);
        }

        public async Task<T> InsertAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = GetColumnsStringSqlServer(allPropertiesExceptKeyAndComputed, columns);

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

            return await connection.QuerySingleOrDefaultAsync<T>(sql, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert entities asynchronously.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Data source connection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesString = GetColumnsStringSqlServer(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = GetColumnsStringSqlServer(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            var properties = keyProperties.First().PropertyType == typeof(Guid) ? allProperties : allPropertiesExceptKeyAndComputed;
            var propertiesString = keyProperties.First().PropertyType == typeof(Guid) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;

            // Open connection.
            connection.Open();

            // Create temporary table to cache resultant bulk copy.
            await connection.ExecuteAsync($@"SELECT TOP 0 {propertiesString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);").ConfigureAwait(false);

            var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, null);

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
        /// Update entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="domain"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            var type = typeof(T);

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                var typeInfo = type.GetTypeInfo();
                var implementsGenericIEnumerableOrIsGenericIEnumerable =
                    typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                    typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                if (implementsGenericIEnumerableOrIsGenericIEnumerable)
                {
                    type = type.GetGenericArguments()[0];
                }
            }

            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var name = domain.TableCache.TableName(type);

            var sb = new StringBuilder();
            sb.Append($"UPDATE {name} SET ");

            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var nonIdProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            nonIdProps.ForEach(x =>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="domain"></param>
        /// <param name="data"></param>
        /// <param name="expressions"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesString = GetColumnsStringSqlServer(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = GetColumnsStringSqlServer(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            var propertiesString = keyProperties.First().PropertyType == typeof(Guid) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;
            var expressionList = new PropertyExpressionList<T>(domain, expressions);

            // Open connection.
            connection.Open();

            // Create temporary table to cache resultant bulk copy.
            await connection.ExecuteAsync($@"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);").ConfigureAwait(false);
            await connection.ExecuteAsync($@"ALTER TABLE {tempToBeInserted} ADD {keyProperties.First().Name} {PropertyCache.GetSqlType(keyProperties.First().PropertyType)}").ConfigureAwait(false);

            try
            {
                // Perform bulk copy
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, null))
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
                                        {string.Join(" AND ", FormatMergeOnMatchList(domain, expressionList))}
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

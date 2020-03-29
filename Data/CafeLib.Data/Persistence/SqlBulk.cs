using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Data.Options;
using Dapper;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Persistence
{
    /// <summary>
    /// Bulk inserts for Dapper
    /// </summary>
    internal static class SqlBulk
    {
        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Update<T>(this IDbConnection connection, Domain domain, T entityToUpdate, IDbTransaction? transaction = null, int? commandTimeout = null) where T : IEntity
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

            var updated = connection.Execute(sb.ToString(), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete<T>(this IDbConnection connection, Domain domain, T entityToDelete, IDbTransaction? transaction = null, int? commandTimeout = null) where T : IEntity
        {
            if (entityToDelete == null)
                throw new ArgumentException("Cannot Delete null Object", nameof(entityToDelete));

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

            var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
            return deleted > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data"></param>
        /// <param name="transaction"></param>
        /// <param name="batchSize"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <returns></returns>
        public static T Insert<T>(this SqlConnection connection, Domain domain, T data, SqlTransaction? transaction = null, int batchSize = 0, int bulkCopyTimeout = 30) where T : IEntity
        {
            try
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

                return connection.QuerySingleOrDefault<T>(sql, data, transaction);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="batchSize">Number of bulk items inserted together, 0 (the default) if all</param>
        /// <param name="bulkCopyTimeout">Number of seconds before bulk command execution timeout, 30 (the default)</param>
        public static int BulkInsert<T>(this SqlConnection connection, Domain domain, IEnumerable<T> data, SqlTransaction? transaction = null, int batchSize = 0, int bulkCopyTimeout = 30) where T : IEntity
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
            connection.Execute($@"SELECT TOP 0 {propertiesString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);", null, transaction);

            var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);

            try
            {
                // Perform bulk copy
                using (var bulkCopy = sqlBulkCopy) //new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = tempToBeInserted;

                    // ReSharper disable once AccessToDisposedClosure
                    properties.ForEach((x, i) => bulkCopy.ColumnMappings.Add(x.Name, columns[x.Name]));
                    using var table = ToDataTable(data, properties).CreateDataReader();
                    bulkCopy.WriteToServer(table);
                }

                // Insert from temporary table to actual table.
                var sql = $@"INSERT INTO {FormatTableName(tableName)}({propertiesString}) SELECT {propertiesString} FROM {tempToBeInserted}";
                var result = connection.Execute(sql, null, transaction);

                connection.Execute($@"DROP TABLE {tempToBeInserted};", null, transaction);
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
                    var items = (object[]?)sortedColumns?.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(sortedColumns);

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
        /// Inserts entities into table <typeparamref name="T"/>s (by default) returns inserted entities.
        /// </summary>
        /// <typeparam name="T">The element type of the array</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="batchSize">Number of bulk items inserted together, 0 (the default) if all</param>
        /// <param name="bulkCopyTimeout">Number of seconds before bulk command execution timeout, 30 (the default)</param>
        /// <returns>Inserted entities</returns>
        public static IEnumerable<T> BulkInsertAndSelect<T>(this SqlConnection connection, Domain domain, IEnumerable<T> data, SqlTransaction? transaction = null, int batchSize = 0, int bulkCopyTimeout = 30) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            if (keyProperties.Count == 0)
            {
                var dataList = data.ToList();
                connection.BulkInsert(domain, dataList, transaction, batchSize, bulkCopyTimeout);
                return dataList;
            }

            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            var keyPropertiesString = GetColumnsStringSqlServer(keyProperties, columns);
            var keyPropertiesInsertedString = GetColumnsStringSqlServer(keyProperties, columns, "inserted.");
            var allPropertiesExceptKeyAndComputedString = GetColumnsStringSqlServer(allPropertiesExceptKeyAndComputed, columns);
            var allPropertiesString = GetColumnsStringSqlServer(allProperties, columns, "target.");

            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);
            var tempInsertedWithIdentity = $"@TempInserted_{tableName}".Replace(".", string.Empty);

            connection.Execute($"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);", null, transaction);

            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = tempToBeInserted;
                bulkCopy.WriteToServer(ToDataTable(data, allPropertiesExceptKeyAndComputed).CreateDataReader());
            }

            var table = string.Join(", ", keyProperties.Select(k => $"[{k.Name}] bigint"));
            var joinOn = string.Join(" AND ", keyProperties.Select(k => $"target.[{k.Name}] = ins.[{k.Name}]"));
            return connection.Query<T>($@"
                DECLARE {tempInsertedWithIdentity} TABLE ({table})
                INSERT INTO {FormatTableName(tableName)}({allPropertiesExceptKeyAndComputedString}) 
                OUTPUT {keyPropertiesInsertedString} INTO {tempInsertedWithIdentity} ({keyPropertiesString})
                SELECT {allPropertiesExceptKeyAndComputedString} FROM {tempToBeInserted}

                SELECT {allPropertiesString}
                FROM {FormatTableName(tableName)} target INNER JOIN {tempInsertedWithIdentity} ins ON {joinOn}

                DROP TABLE {tempToBeInserted};", null, transaction);
        }

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default) asynchronously.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="batchSize">Number of bulk items inserted together, 0 (the default) if all</param>
        /// <param name="bulkCopyTimeout">Number of seconds before bulk command execution timeout, 30 (the default)</param>
        public static async Task BulkInsertAsync<T>(this SqlConnection connection, Domain domain, IEnumerable<T> data, SqlTransaction? transaction = null, int batchSize = 0, int bulkCopyTimeout = 30) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = GetColumnsStringSqlServer(allPropertiesExceptKeyAndComputed, columns);
            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);

            await connection.ExecuteAsync($@"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);", null, transaction);

            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = tempToBeInserted;
                await using var reader = ToDataTable(data, allPropertiesExceptKeyAndComputed).CreateDataReader();
                await bulkCopy.WriteToServerAsync(reader);
            }

            await connection.ExecuteAsync($@"
                INSERT INTO {FormatTableName(tableName)}({allPropertiesExceptKeyAndComputedString}) 
                SELECT {allPropertiesExceptKeyAndComputedString} FROM {tempToBeInserted}

                DROP TABLE {tempToBeInserted};", null, transaction);
        }

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default) asynchronously and returns inserted entities.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="batchSize">Number of bulk items inserted together, 0 (the default) if all</param>
        /// <param name="bulkCopyTimeout">Number of seconds before bulk command execution timeout, 30 (the default)</param>
        /// <returns>Inserted entities</returns>
        public static async Task<IEnumerable<T>> BulkInsertAndSelectAsync<T>(this SqlConnection connection, Domain domain, IEnumerable<T> data, SqlTransaction? transaction = null, int batchSize = 0, int bulkCopyTimeout = 30) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            if (keyProperties.Count == 0)
            {
                var dataList = data.ToList();
                await connection.BulkInsertAsync(domain, dataList, transaction, batchSize, bulkCopyTimeout);
                return dataList;
            }

            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            var keyPropertiesString = GetColumnsStringSqlServer(keyProperties, columns);
            var keyPropertiesInsertedString = GetColumnsStringSqlServer(keyProperties, columns, "inserted.");
            var allPropertiesExceptKeyAndComputedString = GetColumnsStringSqlServer(allPropertiesExceptKeyAndComputed, columns);
            var allPropertiesString = GetColumnsStringSqlServer(allProperties, columns, "target.");

            var tempToBeInserted = $"#TempInsert_{tableName}".Replace(".", string.Empty);
            var tempInsertedWithIdentity = $"@TempInserted_{tableName}".Replace(".", string.Empty);

            await connection.ExecuteAsync($@"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);", null, transaction);

            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = tempToBeInserted;
                await bulkCopy.WriteToServerAsync(ToDataTable(data, allPropertiesExceptKeyAndComputed).CreateDataReader());
            }

            var table = string.Join(", ", keyProperties.Select(k => $"[{k.Name }] bigint"));
            var joinOn = string.Join(" AND ", keyProperties.Select(k => $"target.[{k.Name }] = ins.[{k.Name }]"));
            return await connection.QueryAsync<T>($@"
                DECLARE {tempInsertedWithIdentity} TABLE ({table})
                INSERT INTO {FormatTableName(tableName)}({allPropertiesExceptKeyAndComputedString}) 
                OUTPUT {keyPropertiesInsertedString} INTO {tempInsertedWithIdentity} ({keyPropertiesString})
                SELECT {allPropertiesExceptKeyAndComputedString} FROM {tempToBeInserted}

                SELECT {allPropertiesString}
                FROM {FormatTableName(tableName)} target INNER JOIN {tempInsertedWithIdentity} ins ON {joinOn}

                DROP TABLE {tempToBeInserted};", null, transaction);
        }

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="expressionList"></param>
        /// <param name="batchSize">Number of bulk items inserted together, 0 (the default) if all</param>
        /// <param name="bulkCopyTimeout">Number of seconds before bulk command execution timeout, 30 (the default)</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        public static int BulkUpsert<T>(this SqlConnection connection, Domain domain, IEnumerable<T> data, PropertyExpressionList<T>? expressionList, int batchSize = 0, int bulkCopyTimeout = 30, SqlTransaction? transaction = null) where T : IEntity
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

            // Open connection.
            connection.Open();

            // Create temporary table to cache resultant bulk copy.
            connection.Execute($@"SELECT TOP 0 {allPropertiesExceptKeyAndComputedString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);", null, transaction);
            connection.Execute($@"ALTER TABLE {tempToBeInserted} ADD {keyProperties.First().Name} {PropertyCache.GetSqlType(keyProperties.First().PropertyType)}");

            try
            {
                // Perform bulk copy
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = tempToBeInserted;

                    // ReSharper disable once AccessToDisposedClosure
                    allProperties.ForEach((x, i) => bulkCopy.ColumnMappings.Add(x.Name, columns[x.Name]));
                    using var table = ToDataTable(data, allProperties).CreateDataReader();
                    bulkCopy.WriteToServer(table);
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
                var result = connection.Execute(sql, null, transaction);

                connection.Execute($@"DROP TABLE {tempToBeInserted};", null, transaction);
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

        private static string GetColumnsStringSqlServer(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<string, string> columnNames, string? tablePrefix = null)
        {
            if (tablePrefix == "target.")
            {
                return string.Join(", ", properties.Select(property => $"{tablePrefix}[{columnNames[property.Name]}] as [{property.Name}]"));
            }

            return string.Join(", ", properties.Select(property => $"{tablePrefix}[{columnNames[property.Name]}]"));
        }

        private static DataTable ToDataTable<T>(IEnumerable<T> data, IReadOnlyList<PropertyInfo> properties)
        {
            var typeCasts = new Type?[properties.Count];
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

        private static IEnumerable<string> FormatMergeOnMatchList<T>(Domain domain, PropertyExpressionList<T>? expressionList, string source = "src", string target = "tgt") where T : IEntity
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
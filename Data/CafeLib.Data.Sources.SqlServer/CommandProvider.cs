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
using Dapper;

namespace CafeLib.Data.Sources.SqlServer
{
    internal class CommandProvider : SingletonBase<CommandProvider>, ISqlCommandProcessor
    {
        public T Insert<T>(IDbConnection connection, Domain domain, T data) where T : IEntity
        {
            var sql = InsertSql<T>(domain);
            return connection.QuerySingleOrDefault<T>(sql, data);
        }

        public int Insert<T>(IDbConnection connection, Domain domain, IEnumerable<T> data) where T : IEntity
        {
            return InsertAsync(connection, domain, data).GetAwaiter().GetResult();
        }

        public async Task<T> InsertAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            var sql = InsertSql<T>(domain);
            return await connection.QuerySingleOrDefaultAsync<T>(sql, data).ConfigureAwait(false);
        }


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
            await connection.ExecuteAsync($@"SELECT TOP 0 {propertiesString} INTO {tempToBeInserted} FROM {FormatTableName(tableName)} target WITH(NOLOCK);");

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
                    bulkCopy.WriteToServer(table);
                }

                // Insert from temporary table to actual table.
                var sql = $@"INSERT INTO {FormatTableName(tableName)}({propertiesString}) SELECT {propertiesString} FROM {tempToBeInserted}";
                var result = await connection.ExecuteAsync(sql);

                await connection.ExecuteAsync($@"DROP TABLE {tempToBeInserted};");
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

        public bool Update<T>(IDbConnection connection, Domain domain, T data) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public bool UpdateAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public int Upsert<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions,
            CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(IDbConnection connection, Domain domain, T data) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private static string InsertSql<T>(Domain domain) where T : IEntity
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

            return sql;
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

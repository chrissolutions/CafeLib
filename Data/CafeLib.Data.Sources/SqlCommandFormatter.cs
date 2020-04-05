using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;

namespace CafeLib.Data.Sources
{
    public static class SqlCommandFormatter
    {
        private const string And = " AND ";
        private const string Separator = ", ";

        public static string FormatDeleteStatement<T>(Domain domain) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>().ToList();  //added ToList() due to issue #418, must work on a list copy
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var sqlDelete = new StringBuilder();
            sqlDelete.AppendLine($"DELETE FROM {tableName} WHERE ");

            foreach (var property in keyProperties)
            {
                sqlDelete.AppendLine($"{property.Name} = @{property.Name}");
                sqlDelete.Append(And);
            }
            sqlDelete.Remove(sqlDelete.Length - And.Length, And.Length);

            return sqlDelete.ToString();
        }

        public static string FormatInsertStatement<T>(Domain domain) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();
            var primaryKey = domain.PropertyCache.PrimaryKey<T>();

            var allPropertiesString = FormatColumnsToString(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);

            var properties = primaryKey.PropertyType.IsPrimitive ? allPropertiesExceptKeyAndComputed : allProperties;
            var columnsString = ReferenceEquals(properties, allProperties) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;

            var sqlParameterList = new StringBuilder(null);
            foreach (var property in properties)
            {
                sqlParameterList.Append($"@{columns[property.Name]}{Separator}");
            }
            sqlParameterList.Remove(sqlParameterList.Length - Separator.Length, Separator.Length);

            var sqlInsert = new StringBuilder()
                .AppendLine($"INSERT INTO {tableName}")
                .AppendLine($"({columnsString})")
                .AppendLine("{0}")
                .AppendLine("VALUES")
                .AppendLine($"({sqlParameterList})")
                .AppendLine("{1}");

            return sqlInsert.ToString();
        }

        public static string FormatUpdateStatement<T>(Domain domain, Expression<Func<T, object>>[] expressions = null) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var nonKeyProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            var sqlUpdate = new StringBuilder();
            sqlUpdate.AppendLine($"UPDATE {tableName} SET");
            foreach (var nonKey in nonKeyProps)
            {
                sqlUpdate.AppendLine($"{columns[nonKey.Name]} = @{nonKey.Name}");
                sqlUpdate.Append(Separator);
            }
            sqlUpdate.Remove(sqlUpdate.Length - Separator.Length, Separator.Length);

            sqlUpdate.AppendLine("WHERE");
            var expressionList = new PropertyExpressionList<T>(domain, expressions);

            if (expressionList.Any())
            {
                // ReSharper disable once ImplicitlyCapturedClosure
                expressionList.GetColumnNames().ForEach(x => sqlUpdate.AppendLine($"{x} = @{x}").Append(And));
            }
            else
            {
                keyProperties.ForEach(x => sqlUpdate.AppendLine($"{columns[x.Name]} = @{columns[x.Name]}").Append(And));
            }
            sqlUpdate.Remove(sqlUpdate.Length - And.Length, And.Length);

            return sqlUpdate.ToString();
        }

        public static string FormatColumnsToString(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<string, string> columnNames, string tablePrefix = null)
        {
            var prefix = string.IsNullOrWhiteSpace(tablePrefix) 
                ? string.Empty
                : tablePrefix.EndsWith(".")
                    ? tablePrefix
                    : tablePrefix + ".";

            return string.Join(Separator, 
                                string.IsNullOrWhiteSpace(prefix) 
                                    ? properties.Select(property => $"{prefix}[{columnNames[property.Name]}]") 
                                    : properties.Select(property => $"{prefix}[{columnNames[property.Name]}] as [{property.Name}]"));
        }
    }
}

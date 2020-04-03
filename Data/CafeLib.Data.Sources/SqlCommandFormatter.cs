using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CafeLib.Core.Data;

namespace CafeLib.Data.Sources
{
    public static class SqlCommandFormatter
    {
        public static string FormatDeleteStatement<T>(Domain domain) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>().ToList();  //added ToList() due to issue #418, must work on a list copy
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var sb = new StringBuilder();
            sb.Append($"DELETE FROM {tableName} WHERE ");

            for (var i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties[i];
                sb.Append($"{property.Name} = @{property.Name}");
                if (i < keyProperties.Count - 1)
                {
                    sb.Append(" AND ");
                }
            }

            return sb.ToString();
        }

        public static string FormatInsertStatement<T>(Domain domain) where T : IEntity
        {
            var tableName = domain.TableCache.TableName<T>();
            var allProperties = domain.PropertyCache.TypePropertiesCache<T>();
            var keyProperties = domain.PropertyCache.KeyPropertiesCache<T>();
            var computedProperties = domain.PropertyCache.ComputedPropertiesCache<T>();
            var columns = domain.PropertyCache.GetColumnNamesCache<T>();

            var allPropertiesString = FormatColumnsToString(allProperties, columns);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            var allPropertiesExceptKeyAndComputedString = FormatColumnsToString(allPropertiesExceptKeyAndComputed, columns);

            var properties = keyProperties.First().PropertyType.IsPrimitive ? allPropertiesExceptKeyAndComputed : allProperties;
            var propertiesString = ReferenceEquals(properties, allProperties) ? allPropertiesString : allPropertiesExceptKeyAndComputedString;

            var sbInsert = new StringBuilder()
                .AppendLine($"INSERT INTO {tableName}")
                .AppendLine($"    ({propertiesString})")
                .AppendLine("-- Placeholder01 --");

            var sbParameterList = new StringBuilder(null);
            foreach (var property in properties)
            {
                sbParameterList.Append($"@{property.Name}, ");
            }
            sbParameterList.Remove(sbParameterList.Length - 1, 1);

            sbInsert.AppendLine("VALUES")
                .AppendLine($"    ({sbParameterList})");

            sbInsert.AppendLine()
                .AppendLine("-- Placeholder02 --");

            return sbInsert.ToString();
        }

        public static string FormatColumnsToString(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<string, string> columnNames, string tablePrefix = null)
        {
            var prefix = string.IsNullOrWhiteSpace(tablePrefix) 
                ? string.Empty
                : tablePrefix.EndsWith(".")
                    ? tablePrefix
                    : tablePrefix + ".";

            return string.Join(", ", 
                                string.IsNullOrWhiteSpace(prefix) 
                                    ? properties.Select(property => $"{prefix}[{columnNames[property.Name]}]") 
                                    : properties.Select(property => $"{prefix}[{columnNames[property.Name]}] as [{property.Name}]"));
        }
    }
}

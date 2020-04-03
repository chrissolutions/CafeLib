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

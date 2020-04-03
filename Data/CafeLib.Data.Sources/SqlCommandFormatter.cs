using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CafeLib.Data.Sources
{
    public static class SqlCommandFormatter
    {
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

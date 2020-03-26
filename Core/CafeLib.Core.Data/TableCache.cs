using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Data
{
    /// <summary>
    /// Used to store table names
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    internal class TableCache
    {
        private static string _prefix = string.Empty;
        private static string _suffix = string.Empty;

        private readonly ConcurrentDictionary<Type, string> _tableNames;

        public TableCache(Domain domain)
        {
            _tableNames = new ConcurrentDictionary<Type, string>();
        }

        /// <summary>
        /// Used to setup custom table conventions.
        /// </summary>
        /// <param name="tablePrefix">table name prefix</param>
        /// <param name="tableSuffix">table name suffix</param>
        // ReSharper disable once UnusedMember.Global
        public void SetupConvention(string tablePrefix, string tableSuffix)
        {
            if (_tableNames.Count > 0)
            {
                throw new InvalidConstraintException("TableMapper.SetupConvention called after usage.");
            }

            _prefix = tablePrefix;
            _suffix = tableSuffix;

            _tableNames.Clear();
        }

        public string TableName<T>() where T : IEntity => TableName(typeof(T));

        public string TableName(Type type)
        {
            if (_tableNames.TryGetValue(type, out var name))
            {
                return name;
            }

            var tableAttr = type.GetCustomAttribute<TableAttribute>(false);
            if (tableAttr != null)
            {
                name = tableAttr.Name;
            }
            else
            {
                name = type.IsInterface && type.Name.StartsWith("I")
                    ? type.Name.Substring(1)
                    : type.Name;
                name = $"{_prefix}{name}{_suffix}";
            }

            _tableNames[type] = name;
            return name;
        }
    }
}
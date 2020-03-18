using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace CafeLib.Data.Dto.Cache
{
    /// <summary>
    /// Used to store table names
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class TableCache
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TableNames = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static string _prefix = string.Empty;
        private static string _suffix = string.Empty;

        /// <summary>
        /// Used to setup custom table conventions.
        /// </summary>
        /// <param name="tablePrefix">table name prefix</param>
        /// <param name="tableSuffix">table name suffix</param>
        // ReSharper disable once UnusedMember.Global
        public static void SetupConvention(string tablePrefix, string tableSuffix)
        {
            if (TableNames.Count > 0)
            {
                throw new InvalidConstraintException("TableMapper.SetupConvention called after usage.");
            }

            _prefix = tablePrefix;
            _suffix = tableSuffix;

            TableNames.Clear();
        }

        internal static string TableName<T>() where T : IEntity => TableName(typeof(T));

        internal static string TableName(Type type)
        {
            if (TableNames.TryGetValue(type.TypeHandle, out var name))
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

            TableNames[type.TypeHandle] = name;
            return name;
        }
    }
}
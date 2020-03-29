using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Data
{
    /// <summary>
    /// Used to store table names
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class TableCache
    {
        private readonly Domain _domain;

        private readonly ConcurrentDictionary<Type, string> _tableNames = new ConcurrentDictionary<Type, string>();

        public TableCache(Domain domain)
        {
            _domain = domain;
            SetupConvention(string.Empty, string.Empty);
        }

        /// <summary>
        /// Used to setup custom table conventions.
        /// </summary>
        /// <param name="prefix">table name prefix</param>
        /// <param name="suffix">table name suffix</param>
        public void SetupConvention(string prefix, string suffix)
        {
            _tableNames.Clear();
            _domain.GetEntityTypes().ForEach(x =>
            {
                var tableAttr = x.GetCustomAttribute<TableAttribute>(false);
                var name = tableAttr != null
                    ? tableAttr.Name
                    : $"{prefix}{(x.IsInterface && x.Name.StartsWith("I") ? x.Name.Substring(1) : x.Name)}{suffix}";
                _tableNames[x] = name;
            });
        }

        public string TableName<T>() where T : IEntity => TableName(typeof(T));
        public string TableName(Type type) => _tableNames[type];
    }
}
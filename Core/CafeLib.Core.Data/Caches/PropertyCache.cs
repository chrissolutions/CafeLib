using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Data.Caches
{
    public class PropertyCache
    {
        private readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _keyProperties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        private readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _typeProperties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        private readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _computedProperties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        private readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, string>> _columnNames = new ConcurrentDictionary<Type, IReadOnlyDictionary<string, string>>();

        public PropertyCache(Domain domain)
        {
            var types = domain.GetEntityTypes().ToArray();
            types.ForEach(x =>
            {
                var properties = x.GetProperties().Where(ValidateProperty).ToList();
                _typeProperties[x] = properties;
                _columnNames[x] = GetColumnNames(properties);
            });

            types.ForEach(x =>
            {
                var keyProperties = TypePropertiesCache(x).Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "KeyAttribute")).ToList();
                if (!keyProperties.Any())
                {
                    var idProp = TypePropertiesCache(x).FirstOrDefault(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                    if (idProp != null)
                    {
                        keyProperties.Add(idProp);
                    }
                }

                _keyProperties[x] = keyProperties;
            });

            types.ForEach(x => {
                _computedProperties[x] = TypePropertiesCache(x).Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "ComputedAttribute")).ToList();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IReadOnlyList<PropertyInfo> TypePropertiesCache<T>() where T : IEntity => TypePropertiesCache(typeof(T));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IReadOnlyList<PropertyInfo> TypePropertiesCache(Type type) => _typeProperties[type].ToList();

        /// <summary>
        /// Get the column name map.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>cached map of SQL column names</returns>
        public IReadOnlyDictionary<string, string> GetColumnNamesCache<T>() where T : IEntity => GetColumnNamesCache(typeof(T));

        /// <summary>
        /// Get the column name map.
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns>cached map of SQL column names</returns>
        public IReadOnlyDictionary<string, string> GetColumnNamesCache(Type type) => _columnNames[type];

        /// <summary>
        /// Get the primary key property info.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>name of the primary key</returns>
        public PropertyInfo PrimaryKey<T>() where T : IEntity => KeyPropertiesCache<T>().First();

        /// <summary>
        /// Get the primary key name.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>name of the primary key</returns>
        public string PrimaryKeyName<T>() where T : IEntity => PrimaryKey<T>().Name;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IReadOnlyList<PropertyInfo> KeyPropertiesCache<T>() where T : IEntity => KeyPropertiesCache(typeof(T));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IReadOnlyList<PropertyInfo> KeyPropertiesCache(Type type) => _keyProperties[type].ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IReadOnlyList<PropertyInfo> ComputedPropertiesCache<T>() where T : IEntity => ComputedPropertiesCache(typeof(T));

        public IReadOnlyList<PropertyInfo> ComputedPropertiesCache(Type type) => _computedProperties[type].ToList();


        #region Helpers

        private static IReadOnlyDictionary<string, string> GetColumnNames(IEnumerable<PropertyInfo> props)
        {
            var results = new Dictionary<string, string>();
            foreach (var prop in props)
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                results.Add(prop.Name, columnAttr != null ? columnAttr.Name : prop.Name);
            }

            return results;
        }

        private static bool ValidateProperty(PropertyInfo prop)
        {
            var result = prop.CanWrite;
            result = result && (prop.GetSetMethod(true)?.IsPublic ?? false);
            result = result && (!prop.PropertyType.IsClass || prop.PropertyType == typeof(string) || prop.PropertyType == typeof(byte[]));
            result = result && prop.GetCustomAttributes(true).All(a => a.GetType().Name != "NotMappedAttribute");

            var writeAttribute = prop.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Name == "WriteAttribute");
            if (writeAttribute != null)
            {
                var writeProperty = writeAttribute.GetType().GetProperty("Write");
                if (writeProperty != null && writeProperty.PropertyType == typeof(bool))
                {
                    result = result && (bool)writeProperty.GetValue(writeAttribute);
                }
            }

            return result;
        }

        #endregion
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Dto.Cache
{
    internal static class PropertyCache
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, string>> ColumnNames = new ConcurrentDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, string>>();

        public static IReadOnlyList<PropertyInfo> TypePropertiesCache<T>() where T : IEntity
        {
            var type = typeof(T);

            if (TypeProperties.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps.ToList();
            }

            var properties = type.GetProperties().Where(ValidateProperty).ToList();
            TypeProperties[type.TypeHandle] = properties;
            ColumnNames[type.TypeHandle] = GetColumnNames(properties);

            return properties.ToList();
        }

        /// <summary>
        /// Get the column name map.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>cached map of SQL column names</returns>
        public static IReadOnlyDictionary<string, string> GetColumnNamesCache<T>() where T : IEntity => GetColumnNamesCache(typeof(T));

        /// <summary>
        /// Get the column name map.
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns>cached map of SQL column names</returns>
        public static IReadOnlyDictionary<string, string> GetColumnNamesCache(Type type)
        {
            if (ColumnNames.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps;
            }

            var properties = type.GetProperties().Where(ValidateProperty).ToList();
            TypeProperties[type.TypeHandle] = properties;
            ColumnNames[type.TypeHandle] = GetColumnNames(properties);

            return ColumnNames[type.TypeHandle];
        }

        public static string GetSqlType(Type type)
        {
            return type switch
            {
                { } when type == typeof(int) => "INT",

                { } when type == typeof(long) => "BIGINT",

                { } when type == typeof(Guid) => "UNIQUEIDENTIFER",

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Get the primary key name.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>name of the primary key</returns>
        public static string PrimaryKeyName<T>() where T : IEntity => KeyPropertiesCache<T>().First().Name;

        public static IReadOnlyList<PropertyInfo> KeyPropertiesCache<T>() where T : IEntity
        {
            var type = typeof(T);

            if (KeyProperties.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps.ToList();
            }

            var allProperties = TypePropertiesCache<T>();
            var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "KeyAttribute")).ToList();

            if (keyProperties.Count == 0)
            {
                var idProp = ((List<PropertyInfo>)allProperties).Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null)
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        public static IReadOnlyList<PropertyInfo> ComputedPropertiesCache<T>() where T : IEntity
        {
            var type = typeof(T);

            if (ComputedProperties.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps.ToList();
            }

            var computedProperties = TypePropertiesCache<T>().Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "ComputedAttribute")).ToList();
            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }

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
    }
}
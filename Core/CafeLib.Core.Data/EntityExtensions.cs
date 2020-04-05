using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using CafeLib.Core.Extensions;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Data
{
    public static class EntityExtensions
    {
        public static Type KeyType(this IEntity entity)
        {
            var keyProperty = entity.GetType().GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null);

            return keyProperty?.PropertyType;
        }

        public static object KeyValue(this IEntity entity)
        {
            var keyProperty = entity.GetType().GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null);

            return keyProperty?.GetValue(entity);
        }

        public static object KeyDefaultValue(this IEntity entity)
        {
            return entity.KeyValue().GetType().Default();
        }

        public static bool IsKeyGenerated(this IEntity entity)
        {
            var property = entity.GetType().GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null && x.GetCustomAttribute<DatabaseGeneratedAttribute>() != null);
            return property != null;
        }
    }
}
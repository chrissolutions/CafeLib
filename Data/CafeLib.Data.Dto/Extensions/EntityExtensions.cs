using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace CafeLib.Data.Dto.Extensions
{
    public static class EntityExtensions
    {
        public static string TableName(this IEntity entity)
        {
            var method = typeof(DtoContext).GetMethod("TableName");
            var genericMethod = method.MakeGenericMethod(entity.GetType());
            return genericMethod.Invoke(null, null).ToString();
        }

        public static string KeyName(this IEntity entity)
        {
            var method = typeof(DtoContext).GetMethod("KeyName");
            var genericMethod = method.MakeGenericMethod(entity.GetType());
            return genericMethod.Invoke(null, null).ToString();
        }

        public static PropertyInfo[] KeyProperties(this IEntity entity)
        {
            var method = typeof(DtoContext).GetMethod("KeyProperties");
            var genericMethod = method.MakeGenericMethod(entity.GetType());
            return (PropertyInfo[])genericMethod.Invoke(null, null);
        }

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

        public static bool IsKeyGenerated(this IEntity entity)
        {
            var property = entity.GetType().GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null && x.GetCustomAttribute<DatabaseGeneratedAttribute>() != null);
            return property != null;
        }
    }
}

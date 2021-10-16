﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CafeLib.Data.SqlGenerator.Models
{
    public static class EntityFrameworkEntityTypeExtensions
    {
        private const string TableSchemaAnnoName = "Relational:Schema";

        private const string TableNameAnnoName = "Relational:TableName";

        private const string ColumnNameAnnoName = "Relational:ColumnName";

        public static EntityInfo ToEntityInfo(this IEntityType et)
        {
            var annoName = et.FindAnnotation(TableNameAnnoName);
            if (annoName == null)
            {
                throw new NotSupportedException("Entity must have a table name");
            }

            var annoSchema = et.FindAnnotation(TableSchemaAnnoName);

            var info = new EntityInfo
            {
                Namespace = annoSchema != null ? annoSchema.Value.ToString() : string.Empty,
                EntityName = annoName.Value.ToString(),
                Type = et.ClrType
            };

            return info;
        }

        public static EntityFieldInfo ToEntityFieldInfo(this IProperty p, EntityInfo e, bool isPk = false)
        {
            var annotation = p.FindAnnotation(ColumnNameAnnoName);

            var info = new EntityFieldInfo
            {
                ClrProperty = p.PropertyInfo,
                PropertyName = p.Name,
                DbName = annotation != null ? annotation.Value.ToString() : p.Name,
                ValType = p.ClrType,
                IsPrimaryKey = isPk,
                Entity = e
            };

            if (info.ClrProperty == null)
            {
                var columnName = info.DbName != info.PropertyName
                    ? $" (column {e.EntityName}.{info.DbName})"
                    : string.Empty;

                var msg = $@"Property '{e.Type.Name}.{info.PropertyName}'{columnName} does not have meta data 'PropertyInfo'.
Please check if it is setup correctly in the data context. EF may generate this column by convention if it is associated with relations.";

                throw new Exception(msg);
            }

            return info;
        }

        public static List<EntityFieldInfo> ToEntityFieldInfo(this IKey k, EntityInfo e)
        {
            return k.Properties.Select(p => p.ToEntityFieldInfo(e, true)).ToList();
        }
    }
}
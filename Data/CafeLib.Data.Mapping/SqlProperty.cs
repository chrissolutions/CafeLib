using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CafeLib.Core.Extensions;

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// Serves as the base class for all property mapping implementations.
    /// </summary>
    internal class SqlProperty : ISqlProperty, IMapper
    {
        public bool IsKey { get; }

        public IEnumerable<string> Tables { get; }

        public IEnumerable<SqlSchema> Schemas { get; }

        public PropertyInfo PropertyInfo { get; }

        public bool Ignored { get; private set; }

        public Func<object, object> ConvertFrom { get; private set; }

        public Func<object, object> ConvertTo { get; private set; }

        public SqlProperty(PropertyInfo info)
        {
            PropertyInfo = info;
            Schemas = new List<SqlSchema>();
            Tables = new HashSet<string>();

            IsKey = info.GetCustomAttribute<KeyAttribute>() != null;
            if (info.IsDefined(typeof(SchemaAttribute)))
            {
                info.GetCustomAttributes<SchemaAttribute>().ForEach(x =>
                {
                    ((List<SqlSchema>)Schemas).Add(new SqlSchema(x));
                    ((HashSet<string>)Tables).Add(x.Table);
                });
            }
            else if (info.IsDefined(typeof(ColumnAttribute)))
            {
                info.GetCustomAttributes<ColumnAttribute>().ForEach(x =>
                {
                    ((List<SqlSchema>)Schemas).Add(new SqlSchema(x));
                });
            }
        }

        /// <summary>
        /// Add schema to sql property cache.
        /// </summary>
        /// <param name="table">table information</param>
        /// <param name="column">column information</param>
        /// <param name="isKey">is the column a key</param>
        /// <returns>sql property</returns>
        public ISqlProperty AddSchema(string table, string column, bool isKey = false)
        {
            ((List<SqlSchema>)Schemas).Add(new SqlSchema(table, column, isKey));
            ((HashSet<string>)Tables).Add(table);
            return this;
        }

        /// <summary>
        /// Marks the current property as ignored, resulting in the property not being mapped by Dapper.
        /// </summary>
        /// <returns>The current PropertyMap instance. This enables a fluent API.</returns>
        public ISqlProperty Ignore()
        {
            Ignored = true;
            return this;
        }

        /// <summary>
        /// Set from property type to target converter.
        /// </summary>
        /// <typeparam name="TFrom">from property type</typeparam>
        /// <param name="func">from converter</param>
        /// <returns>sql property</returns>
        public IMapper Convert<TFrom>(Func<TFrom, object> func)
        {
            ConvertFrom = o => func.Invoke((TFrom)o);
            return this;
        }

        /// <summary>
        /// Set to property type from source converter.
        /// </summary>
        /// <typeparam name="TFrom">from property type</typeparam>
        /// <typeparam name="TTo">to property type</typeparam>
        /// <param name="func">to converter</param>
        /// <returns>sql property</returns>
        public IMapper Convert<TFrom, TTo>(Func<TFrom, TTo> func)
        {
            ConvertTo = o => func.Invoke((TFrom)o);
            return this;
        }
    }
}
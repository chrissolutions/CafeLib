using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Dto
{
    public abstract class DtoContext
    {
        #region Methods

        /// <summary>
        /// Get the context entity model types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetEntityTypes()
        {
            return FindEntityTypes(x => x != typeof(IEntity) && x != typeof(IQuery) && typeof(IEntity).IsAssignableFrom(x));
        }

        /// <summary>
        /// Find the context entities model types.
        /// </summary>
        /// <param name="filter">filter applied to the entity types</param>
        /// <returns></returns>
        protected virtual IEnumerable<Type> FindEntityTypes(Func<Type, bool> filter)
        {
            return GetType().Assembly
                .GetTypes()
                .Where(filter)
                .ToArray();
        }

        #endregion

        #region Static Methods

        public static string TableName<T>() where T : IEntity
        {
            return typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
        }

        public static string KeyName<T>() where T : IEntity
        {
            return typeof(T).GetProperties()
                .Where(x => x.GetCustomAttribute<KeyAttribute>() != null)
                .Select(x => x.GetCustomAttribute<ColumnAttribute>())
                .Select(x => x.Name)
                .FirstOrDefault();
        }

        public static PropertyInfo[] KeyProperties<T>() where T : class, IEntity
        {
            return typeof(T).GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null).ToArray();
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CafeLib.Data.Dto;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// Represents a typed mapping of an entity.
    /// This serves as a marker interface for generic type inference.
    /// </summary>
    public interface IEntityMap<T> where T : IEntity
    {
        /// <summary>
        /// Names of tables supported by the entity.
        /// </summary>
        IEnumerable<string> Tables { get; }

        /// <summary>
        /// Properties supported by the entity.
        /// </summary>
        IEnumerable<ISqlProperty> Properties { get; }

        /// <summary>
        /// Find Sql property associated with the property info.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        ISqlProperty Find(PropertyInfo prop);

        /// <summary>
        /// Finds the sql properties associated with the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>Collection of sql properties</returns>
        IEnumerable<ISqlProperty> FindTableProperties(string tableName);

        /// <summary>
        /// Map property info.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        IMapper Map(PropertyInfo propertyInfo);

        /// <summary>
        /// Map entity expression.
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <typeparam name="TU">type of mapped item</typeparam>
        /// <param name="expression">mapping expression</param>
        /// <returns>sql property</returns>
        IMapper Map<TU>(Expression<Func<T, TU>> expression);
    }
}
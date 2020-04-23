using System;
using System.Collections.Generic;
using System.Reflection;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// Represents the mapping of a property.
    /// </summary>
    public interface ISqlProperty : IMapper
    {
        /// <summary>
        /// Determine whether the property is key.
        /// </summary>
        bool IsKey { get; }

        /// <summary>
        /// Get the tables associated with this property.
        /// </summary>
        IEnumerable<string> Tables { get; }

        /// <summary>
        /// Gets the name of the column in the data store.
        /// </summary>
        IEnumerable<SqlSchema> Schemas { get; }

        /// <summary>
        /// Gets the PropertyInfo object for the current property.
        /// </summary>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets a value indicating whether the property should be ignored when mapping.
        /// </summary>
        bool Ignored { get; }

        /// <summary>
        /// 
        /// </summary>
        Func<object, object> ConvertFrom { get; }

        /// <summary>
        /// 
        /// </summary>
        Func<object, object> ConvertTo { get; }

        /// <summary>
        /// Add schema to sql property cache.
        /// </summary>
        /// <param name="table">table information</param>
        /// <param name="column">column information</param>
        /// <param name="isKey">is the column a key</param>
        /// <returns>sql property</returns>
        ISqlProperty AddSchema(string table, string column, bool isKey = false);

        /// <summary>
        /// Ignore sql property.
        /// </summary>
        /// <returns>sql property</returns>
        ISqlProperty Ignore();
    }
}
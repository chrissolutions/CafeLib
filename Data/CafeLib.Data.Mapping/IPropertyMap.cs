using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// An interface used to map a property converter.
    /// </summary>
    public interface IPropertyMap<TModel> :IEnumerable<IMapper> where TModel : class
    {
        /// <summary>
        /// Map property info.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        IMapper Map(PropertyInfo propertyInfo);

        /// <summary>
        /// Map entity expression.
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <returns>sql property</returns>
        /// <returns></returns>
        IMapper Map<TProperty>(Expression<Func<TModel, TProperty>> expression);
    }
}
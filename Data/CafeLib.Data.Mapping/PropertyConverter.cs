using System;
using System.Reflection;

namespace CafeLib.Data.Mapping
{
    internal class PropertyConverter : IMapper
    {
        /// <summary>
        /// PropertyConverter constructor.
        /// </summary>
        /// <param name="propertyInfo"></param>
        internal PropertyConverter(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        internal PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// ToProperty conversion.
        /// </summary>
        internal object ToProperty { get; private set; }

        /// <summary>
        /// ToObject conversion.
        /// </summary>
        internal object ToObject { get; private set; }

        /// <summary>
        /// Convert property to object.
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <param name="func">mapping function</param>
        /// <returns>mapper</returns>
        public IMapper Convert<TProperty>(Func<TProperty, object> func)
        {
            ToObject = func;
            return this;
        }

        /// <summary>
        /// Convert from value to property.
        /// </summary>
        /// <typeparam name="TFrom">type of from value</typeparam>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <param name="func">mapping function</param>
        /// <returns>mapper</returns>
        public IMapper Convert<TFrom, TProperty>(Func<TFrom, TProperty> func)
        {
            ToProperty = func;
            return this;
        }
    }
}
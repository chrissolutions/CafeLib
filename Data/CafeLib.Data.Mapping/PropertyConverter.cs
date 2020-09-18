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
        /// ToOutput conversion.
        /// </summary>
        internal object ToOutput { get; private set; }

        /// <summary>
        /// Convert property to object.
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <param name="func">mapping function</param>
        /// <returns>mapper</returns>
        public IMapper Convert<TProperty>(Func<TProperty, object> func)
        {
            ToOutput = func;
            return this;
        }

        /// <summary>
        /// Convert from value to property.
        /// </summary>
        /// <typeparam name="TInput">input type</typeparam>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <param name="func">mapping function</param>
        /// <returns>mapper</returns>
        public IMapper Convert<TInput, TProperty>(Func<TInput, TProperty> func)
        {
            ToProperty = func;
            return this;
        }

        /// <summary>
        /// Convert from value to property.
        /// </summary>
        /// <typeparam name="TInput">input type</typeparam>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <param name="func">mapping function</param>
        /// <returns>mapper</returns>
        public IMapper From<TInput, TProperty>(Func<TInput, TProperty> func)
        {
            ToProperty = func;
            return this;
        }

        /// <summary>
        /// Convert property to object.
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <typeparam name="TOutput">output type</typeparam>
        /// <param name="func">mapping function</param>
        /// <returns>mapper</returns>
        public IMapper To<TProperty, TOutput>(Func<TProperty, TOutput> func)
        {
            ToOutput = func;
            return this;
        }
    }
}
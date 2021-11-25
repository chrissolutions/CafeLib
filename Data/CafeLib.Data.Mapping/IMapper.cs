using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// An interface used to convert property values.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Set from property type to target converter.
        /// </summary>
        /// <typeparam name="TProperty">from property type</typeparam>
        /// <param name="func">from converter</param>
        /// <returns>mapper interface</returns>
        IMapper Convert<TProperty>(Func<TProperty, object> func);

        /// <summary>
        /// Set to property type from source converter.
        /// </summary>
        /// <typeparam name="TInput">input type</typeparam>
        /// <typeparam name="TProperty">to property type</typeparam>
        /// <param name="func">to converter</param>
        /// <returns>mapper interface</returns>
        IMapper Convert<TInput, TProperty>(Func<TInput, TProperty> func);

        /// <summary>
        /// Convert from input type to the property type.
        /// </summary>
        /// <typeparam name="TInput">input type</typeparam>
        /// <typeparam name="TProperty">to property type</typeparam>
        /// <param name="func">to converter</param>
        /// <returns>mapper interface</returns>
        IMapper From<TInput, TProperty>(Func<TInput, TProperty> func);


        /// <summary>
        /// Convert to output type from the property type.
        /// </summary>
        /// <typeparam name="TProperty">from property type</typeparam>
        /// <typeparam name="TOutput">to output type</typeparam>
        /// <param name="func">from converter</param>
        /// <returns>mapper interface</returns>
        IMapper To<TProperty, TOutput>(Func<TProperty, TOutput> func);
    }
}
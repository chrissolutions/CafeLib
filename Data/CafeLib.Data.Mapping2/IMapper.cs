using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// Represents a typed mapping of an entity.
    /// This serves as a marker interface for generic type inference.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Set from property type to target converter.
        /// </summary>
        /// <typeparam name="TFrom">from property type</typeparam>
        /// <param name="func">from converter</param>
        /// <returns>sql property</returns>
        IMapper Convert<TFrom>(Func<TFrom, object> func);

        /// <summary>
        /// Set to property type from source converter.
        /// </summary>
        /// <typeparam name="TFrom">from property type</typeparam>
        /// <typeparam name="TTo">to property type</typeparam>
        /// <param name="func">to converter</param>
        /// <returns>sql property</returns>
        IMapper Convert<TFrom, TTo>(Func<TFrom, TTo> func);
    }
}
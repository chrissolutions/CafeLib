using System;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.IoC
{
    public interface IServiceResolver : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns>the service object</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve the service.
        /// </summary>
        /// <param name="type">service type</param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns>the service object</returns>
        bool TryResolve<T>(out T value) where T : class;

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <returns>the service object</returns>
        bool TryResolve(Type type, out object value);
    }
}
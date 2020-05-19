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
    }
}
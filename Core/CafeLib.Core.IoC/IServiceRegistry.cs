using System;
using Microsoft.Extensions.Logging;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.IoC
{
    public interface IServiceRegistry
    {
        /// <summary>
        /// Logging service registration.
        /// </summary>
        /// <param name="configuration">configuration action</param>
        /// <returns>service registry interface</returns>
        IServiceRegistry AddLogging(Action<ILoggingBuilder> configuration);

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        IServiceRegistry AddScoped<TService>() where TService : class;

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        IServiceRegistry AddScoped<TService, TImpl>() where TService : class where TImpl : class, TService;

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        IServiceRegistry AddScoped<TService>(Func<IServiceResolver, TService> factory) where TService : class;

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        IServiceRegistry AddSingleton<TService>() where TService : class;

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        IServiceRegistry AddSingleton<TService, TImpl>() where TService : class where TImpl : class, TService;

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        IServiceRegistry AddSingleton<TService>(Func<IServiceResolver, TService> factory) where TService : class;

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        IServiceRegistry AddTransient<TService>() where TService : class;

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        IServiceRegistry AddTransient<TService, TImpl>() where TService : class where TImpl : class, TService;

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        IServiceRegistry AddTransient<TService>(Func<IServiceResolver, TService> factory) where TService : class;

        /// <summary>
        /// Obtain the service resolver.
        /// </summary>
        /// <returns>service resolve</returns>
        IServiceResolver GetResolver();
    }
}

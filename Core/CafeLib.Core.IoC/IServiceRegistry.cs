using System;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.IoC
{
    public interface IServiceRegistry : IDisposable
    {
        /// <summary>
        /// Logging service registration.
        /// </summary>
        /// <param name="configuration">configuration action</param>
        /// <returns>service registry interface</returns>
        IServiceRegistry AddLogging(Action<ILoggingBuilder> configuration);

        /// <summary>
        /// Scoped service registration.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>service registry interface</returns>
        IServiceRegistry AddScoped<TService, TImpl>() where TService : class where TImpl : class, TService;

        /// <summary>
        /// Scoped service registration.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        IServiceRegistry AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        IServiceRegistry AddSingleton<TService, TImpl>() where TService : class where TImpl : class, TService;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        IServiceRegistry AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        IServiceRegistry AddTransient<TService, TImpl>() where TService : class where TImpl : class, TService;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        IServiceRegistry AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        /// <summary>
        /// Obtain the service resolver.
        /// </summary>
        /// <returns>service resolve</returns>
        IServiceResolver GetResolver();
    }
}

using System;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.Services;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Startup
{
    internal sealed class CafeRegistry : IServiceRegistry, IServiceResolver
    {
        private readonly IServiceRegistry _serviceRegistry;

        /// <summary>
        /// CafeRegistry constructor.
        /// </summary>
        internal CafeRegistry()
        {
            var mobileService = new Lazy<MobileService>(() => new MobileService(GetResolver()));
            _serviceRegistry = IocFactory.CreateRegistry()
                .AddEventService()
                .AddDictionaryService()
                .AddSingleton(x => mobileService.Value as IServiceResolver)
                .AddSingleton(x => mobileService.Value as IPageService)
                .AddSingleton(x => mobileService.Value as INavigationService)
                .AddSingleton(x => mobileService.Value as IDeviceService)
                .AddSingleton(x => mobileService.Value as IAlertService);
        }

        ///// <summary>
        ///// Logging service registration.
        ///// </summary>
        ///// <param name="configuration">configuration action</param>
        ///// <returns>service registry interface</returns>
        //public IServiceRegistry AddLogging(Action<ILoggingBuilder> configuration)
        //{
        //    return _serviceRegistry.AddLogging(configuration);
        //}

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddScoped<TService>() where TService : class
        {
            return _serviceRegistry.AddScoped<TService>();
        }

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddScoped<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            return _serviceRegistry.AddScoped<TService, TImpl>();
        }

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        public IServiceRegistry AddScoped<TService>(Func<IServiceResolver, TService> factory) where TService : class
        {
            return _serviceRegistry.AddScoped(factory);
        }

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddSingleton<TService>() where TService : class
        {
            return _serviceRegistry.AddSingleton<TService>();
        }

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddSingleton<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            return _serviceRegistry.AddSingleton<TService, TImpl>();
        }

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        public IServiceRegistry AddSingleton<TService>(Func<IServiceResolver, TService> factory) where TService : class
        {
            return _serviceRegistry.AddSingleton(factory);
        }

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddTransient<TService>() where TService : class
        {
            return _serviceRegistry.AddTransient<TService>();
        }

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddTransient<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            return _serviceRegistry.AddTransient<TService, TImpl>();
        }

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        public IServiceRegistry AddTransient<TService>(Func<IServiceResolver, TService> factory) where TService : class
        {
            return _serviceRegistry.AddTransient(factory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IServiceResolver GetResolver()
        {
            return _serviceRegistry.GetResolver();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            GetResolver().Dispose();
        }

        /// <summary>
        /// Resolve the dependency.
        /// </summary>
        /// <typeparam name="T">dependency type</typeparam>
        /// <returns>instance of dependency type</returns>
        public T Resolve<T>() where T : class
        {
            return GetResolver().Resolve<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return GetResolver().Resolve(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryResolve<T>(out T value) where T : class
        {
            return GetResolver().TryResolve(out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryResolve(Type type, out object value)
        {
            return GetResolver().TryResolve(type, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return GetResolver().GetService(serviceType);
        }
    }
}

using System;
using CafeLib.Core.IoC;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace CafeLib.Mobile.Services
{
    public sealed class MobileServices : IServiceRegistry, IServiceResolver
    {
        private readonly IServiceRegistry _serviceRegistry;

        /// <summary>
        /// ServiceProvider instance constructor.
        /// </summary>
        public MobileServices()
        {
            var mobileService = new MobileService();
            _serviceRegistry = IocFactory.CreateRegistry()
                .AddSingleton<IServiceResolver>(x => this)
                .AddSingleton(x => mobileService as IPageService)
                .AddSingleton(x => mobileService as INavigationService)
                .AddSingleton(x => mobileService as IDeviceService);
        }

        [UsedImplicitly]
        internal IPageService PageService => GetResolver().Resolve<IPageService>();

        [UsedImplicitly]
        internal INavigationService NavigationService => GetResolver().Resolve<INavigationService>();

        [UsedImplicitly]
        internal IDeviceService DeviceService => GetResolver().Resolve<IDeviceService>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration">configuration action</param>
        /// <returns></returns>
        public IServiceRegistry AddLogging(Action<ILoggingBuilder> configuration)
        {
            return _serviceRegistry.AddLogging(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        public IServiceRegistry AddScoped<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            return _serviceRegistry.AddScoped<TService, TImpl>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IServiceRegistry AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            return _serviceRegistry.AddScoped(factory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        public IServiceRegistry AddSingleton<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            return _serviceRegistry.AddSingleton<TService, TImpl>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IServiceRegistry AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            return _serviceRegistry.AddSingleton(factory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        public IServiceRegistry AddTransient<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            return _serviceRegistry.AddTransient<TService, TImpl>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IServiceRegistry AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class
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
        /// 
        /// </summary>
        public void Dispose()
        {
            _serviceRegistry.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [UsedImplicitly]
        public T Resolve<T>() where T : class
        {
            return GetResolver().Resolve<T>();
        }
    }
}

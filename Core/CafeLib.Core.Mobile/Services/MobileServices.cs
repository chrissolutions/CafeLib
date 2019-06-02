using System;
using CafeLib.Core.IoC;
using CafeLib.Core.Support;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Mobile.Services
{
    public sealed class MobileServices : SingletonBase<MobileServices>, IServiceRegistry
    {
        #region Private Variables

        private readonly IServiceRegistry _serviceRegistry;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceProvider instance constructor.
        /// </summary>
        private MobileServices()
        {
            var mobileService = new MobileService();
            _serviceRegistry = IocFactory.CreateRegistry()
                .AddSingleton(x => mobileService as IPageService)
                .AddSingleton(x => mobileService as INavigationService)
                .AddSingleton(x => mobileService as IDeviceService);
        }

        #endregion

        #region Automatic Properties

        internal static IPageService PageService => Instance.GetResolver().Resolve<IPageService>();

        internal static INavigationService NavigationService => Instance.GetResolver().Resolve<INavigationService>();

        internal static IDeviceService DeviceService => Instance.GetResolver().Resolve<IDeviceService>();

        #endregion

        #region Methods

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

        #endregion

        #region Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [UsedImplicitly]
        public static T Resolve<T>() where T : class, IServiceProvider
        {
            return Instance.GetResolver().Resolve<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public static void ShutDown()
        {
            Instance.Dispose();
        }

        #endregion
    }
}

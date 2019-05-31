using System;
using CafeLib.Core.IoC;
using CafeLib.Core.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CafeLib.Mobile.Core.Services
{
    public sealed class MobileServices : SingletonBase<MobileServices>, IServiceRegistry
    {
        #region Private Variables

        private readonly ServiceRegistry _serviceRegistry;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceProvider instance constructor.
        /// </summary>
        private MobileServices()
        {
            _serviceRegistry = new ServiceRegistry();

            var mobileService = new MobileService();
            _serviceRegistry.AddSingleton(x => mobileService as IPageService);
            _serviceRegistry.AddSingleton(x => mobileService as INavigationService);
            _serviceRegistry.AddSingleton(x => mobileService as IDeviceService);
        }

        #endregion

        #region Automatic Properties

        internal static IPageService PageService => Instance._serviceRegistry.Resolve<IPageService>();

        internal static INavigationService NavigationService => Instance._serviceRegistry.Resolve<INavigationService>();

        internal static IDeviceService DeviceService => Instance._serviceRegistry.Resolve<IDeviceService>();

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class, IServiceProvider
        {
            return _serviceRegistry.ServiceProvider.GetService<T>();
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
        /// Resolve the specified service type.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns>the service object</returns>
        public static T GetService<T>() where T : class, IServiceProvider
        {
            return Instance.Resolve<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShutDown()
        {
            Instance.Dispose();
        }

        #endregion
    }
}

using System;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.IoC
{
    public class ServiceRegistry : IServiceRegistry
    {
        #region Private Variables

        private readonly ServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;
        private bool _disposed;

        #endregion

        #region Constructors

        public ServiceRegistry()
        {
            _serviceCollection = new ServiceCollection();
        }

        #endregion

        #region Properties

        public IServiceProvider ServiceProvider => _serviceProvider ?? (_serviceProvider = _serviceCollection.BuildServiceProvider());

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration">configuration action</param>
        /// <returns></returns>
        public IServiceRegistry AddLogging(Action<ILoggingBuilder> configuration)
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddLogging(configuration);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        public IServiceRegistry AddScoped<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddScoped<TService, TImpl>();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IServiceRegistry AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddScoped(factory.Invoke);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        public IServiceRegistry AddSingleton<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddSingleton<TService, TImpl>();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IServiceRegistry AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddSingleton(factory.Invoke);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        public IServiceRegistry AddTransient<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddTransient<TService, TImpl>();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IServiceRegistry AddTransient<T>(Func<IServiceProvider, T> factory) where T : class
        {
            if (_serviceProvider != null) throw new InvalidOperationException(nameof(_serviceProvider));
            _serviceCollection.AddTransient(factory.Invoke);
            return this;
        }

        public T Resolve<T>() where T : class, IServiceProvider
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Shuts down.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
        }

        #endregion

        #region Helpers

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _serviceProvider?.GetServices<IServiceProvider>().ForEach(x => (x as IDisposable)?.Dispose());
        }

        #endregion
    }
}

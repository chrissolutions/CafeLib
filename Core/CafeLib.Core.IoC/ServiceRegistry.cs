using System;
using CafeLib.Core.IoC.LightInject;
using ServiceContainer = CafeLib.Core.IoC.LightInject.ServiceContainer;

namespace CafeLib.Core.IoC
{
    public class ServiceRegistry : IServiceRegistry, IServiceResolver
    {
        #region Private Variables

        private readonly ServiceContainer _serviceContainer;
        private IServiceProvider _serviceProvider;
        private bool _disposed;

        #endregion

        #region Constructors

        internal ServiceRegistry()
        {
            _serviceContainer = new ServiceContainer();
            _serviceContainer.Register<IServiceResolver>(x => this);
        }

        #endregion

        #region Properties

        public IServiceProvider ServiceProvider => _serviceProvider ??= this;

        #endregion

        #region Methods

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddScoped<TService>() where TService : class
        {
            _serviceContainer.RegisterScoped<TService>();
            return this;
        }

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddScoped<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            _serviceContainer.RegisterScoped<TService, TImpl>();
            return this;
        }

        /// <summary>
        /// Register service of scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        public IServiceRegistry AddScoped<TService>(Func<IServiceResolver, TService> factory) where TService : class
        {
            _serviceContainer.RegisterScoped(x => factory.Invoke(this));
            return this;
        }

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddSingleton<TService>() where TService : class
        {
            _serviceContainer.RegisterSingleton<TService>();
            return this;
        }

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddSingleton<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            _serviceContainer.RegisterSingleton<TService, TImpl>();
            return this;
        }

        /// <summary>
        /// Register service as a singleton.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        public IServiceRegistry AddSingleton<TService>(Func<IServiceResolver, TService> factory) where TService : class
        {
            _serviceContainer.RegisterSingleton(x => factory.Invoke(this));
            return this;
        }

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddTransient<TService>() where TService : class
        {
            _serviceContainer.Register<TService>();
            return this;
        }

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImpl">service implementation type</typeparam>
        /// <returns>service registry</returns>
        public IServiceRegistry AddTransient<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            _serviceContainer.Register<TService, TImpl>();
            return this;
        }

        /// <summary>
        /// Register service as transient.
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="factory">service factory</param>
        /// <returns>service registry</returns>
        public IServiceRegistry AddTransient<TService>(Func<IServiceResolver, TService> factory) where TService : class
        {
            _serviceContainer.Register(x => factory.Invoke(this));
            return this;
        }

        public IServiceResolver GetResolver()
        {
            return this;
        }

        /// <summary>
        /// Get the service
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <returns>service instance</returns>
        public object GetService(Type serviceType)
        {
            return Resolve(serviceType);
        }

        /// <summary>
        /// Resolve the service.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns>service instance</returns>
        public T Resolve<T>() where T : class
        {
            return (T)_serviceContainer.GetInstance(typeof(T));
        }

        /// <summary>
        /// Resolve the service. 
        /// </summary>
        /// <param name="type">service type</param>
        /// <returns>service instance</returns>
        public object Resolve(Type type)
        {
            return _serviceContainer.GetInstance(type);
        }

        /// <summary>
        /// Resolve the service.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns>service instance</returns>
        public bool TryResolve<T>(out T value) where T : class
        {
            try
            {
                value = Resolve<T>();
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Resolve the service.
        /// </summary>
        /// <param name="type">service type</param>
        /// <param name="value">service instance</param>
        /// <returns>service instance</returns>
        public bool TryResolve(Type type, out object value)
        {
            try
            {
                value = ServiceProvider.GetService(type);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Dispose service registry.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Dispose service provider
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _serviceContainer?.Dispose();
        }

        #endregion
    }
}

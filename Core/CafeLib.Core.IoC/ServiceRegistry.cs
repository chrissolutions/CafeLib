using System;
using System.Collections.Concurrent;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.IoC
{
    internal sealed class ServiceRegistry : ServiceBase, IDisposable
    {
        #region Private Variables

        private readonly ConcurrentDictionary<Type, ServiceFactory> _factories;
        private readonly ConcurrentDictionary<Type, object> _services;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceProvider instance constructor.
        /// </summary>
        public ServiceRegistry()
        {
            // Create dictionaries.
            _factories = new ConcurrentDictionary<Type, ServiceFactory>();
            _services = new ConcurrentDictionary<Type, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of the service type.
        /// </summary>
        /// <param name='p'>
        /// P.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public T Create<T>(params object[] p) where T : IServiceProvider
        {
            return (T)_factories[typeof(T)](p);
        }

        /// <summary>
        /// Returns the service.
        /// </summary>
        /// <param name="serviceType">the service type</param>
        /// <returns></returns>
        public override object GetService(Type serviceType)
        {
            return _services.TryGetValue(serviceType, out var value) ? value : null;
        }

        /// <summary>
        /// Register the service factory.
        /// </summary>
        /// <typeparam name='T'> The 1st type parameter.</typeparam>
        /// <param name="factory">service factory</param>
        public void Register<T>(ServiceFactory factory)
        {
            _factories.AddOrUpdate(typeof(T), factory, (k, v) => factory);
        }

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <param name='p'>
        /// P.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public T Resolve<T>(params object[] p) where T : IServiceProvider
        {
            return (T)_services.GetOrAdd(typeof(T), Create<T>(p));
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
            _services.ForEach(x => (x.Value as IDisposable)?.Dispose());
        }

        #endregion
    }
}

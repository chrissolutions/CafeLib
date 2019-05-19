using System;
using System.Threading.Tasks;
using CafeLib.Core.Support;

namespace CafeLib.Core.IoC
{
    public sealed class ServiceProvider : SingletonBase<ServiceProvider>
    {
        #region Private Variables

        private readonly ServiceRegistry _serviceRegistry;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceProvider instance constructor.
        /// </summary>
        private ServiceProvider()
        {
            _serviceRegistry = new ServiceRegistry();
            _serviceRegistry.Register<IEventService>(p => new EventService());
            _serviceRegistry.Register<IPropertyService>(p => new PropertyService());
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Asychronous initialization.
        /// </summary>
        /// <param name="initFunc"></param>
        /// <returns></returns>
        public static async Task InitAsync(Func<Task> initFunc)
        {
            initFunc?.Invoke();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Register the specified service factory.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <param name="factory">service factory</param>
        public static void Register<T>(ServiceFactory<T> factory) where T : class, IServiceProvider
        {
            Instance._serviceRegistry.Register(factory);
        }

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <param name="p">service factory parameters</param>
        /// <returns>the service object</returns>
        public static T Resolve<T>(params object[] p) where T : class, IServiceProvider
        {
            return Instance._serviceRegistry.Resolve<T>(p);
        }

        /// <summary>
        /// Shuts down the all services.
        /// </summary>
        public static void ShutDown()
        {
            Instance._serviceRegistry.Dispose();
        }

        #endregion
    }
}

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
        public static Task InitAsync(Func<Task> initFunc)
        {
            return initFunc?.Invoke();
        }

        /// <summary>
        /// Register the specified service creator.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        public static void Register<T>() where T : IPropertyService
        {
            Instance._serviceRegistry.Register<T>(p => new PropertyService());
        }

        /// <summary>
        /// Register the specified service facory.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <param name="factory">service factory</param>
        public static void Register<T>(ServiceFactory factory)
        {
            Instance._serviceRegistry.Register<T>(factory);
        }

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <param name="p">factory parameters</param>
        /// <returns></returns>
        public static T Resolve<T>(params object[] p)
        {
            return Instance._serviceRegistry.Resolve<T>(p);
        }

        /// <summary>
        /// Shuts down.
        /// </summary>
        public static void ShutDown()
        {
            Instance._serviceRegistry.Dispose();
        }

        #endregion
    }
}

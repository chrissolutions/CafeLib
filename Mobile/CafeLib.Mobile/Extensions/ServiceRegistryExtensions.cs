using System.Reflection;
using CafeLib.Core.Collections;
using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Attributes;
using CafeLib.Mobile.ViewModels;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Extensions
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Add the property service to the service registry.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <returns></returns>
        public static IServiceRegistry AddDictionaryService(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.AddSingleton<IDictionaryService>(x => DictionaryService.Current);
            return serviceRegistry;
        }

        /// <summary>
        /// Add the event service to the service registry.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <returns></returns>
        public static IServiceRegistry AddEventService(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.AddSingleton<IEventService>(x => EventService.Current);
            return serviceRegistry;
        }

        /// <summary>
        /// Add view model type as singleton service registry. 
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="serviceRegistry">service registry</param>
        /// <param name="transient">register as transient if true; otherwise as singleton</param>
        /// <returns>service registry</returns>
        public static IServiceRegistry AddViewModel<T>(this IServiceRegistry serviceRegistry, bool transient = false) where T : BaseViewModel
        {
            var attr = typeof(T).GetTypeInfo().GetCustomAttribute<TransientAttribute>();
            transient = attr != null || transient;
            return !transient ? serviceRegistry.AddViewModelSingleton<T>() : serviceRegistry.AddViewModelTransient<T>();
        }

        /// <summary>
        /// Add singleton view model type to service registry. 
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="serviceRegistry">service registry</param>
        /// <returns>service registry</returns>
        public static IServiceRegistry AddViewModelSingleton<T>(this IServiceRegistry serviceRegistry) where T : BaseViewModel
        {
            return serviceRegistry.AddSingleton<T>();
        }

        /// <summary>
        /// Add transient view model type to service registry. 
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="serviceRegistry">service registry</param>
        /// <returns>service registry</returns>
        public static IServiceRegistry AddViewModelTransient<T>(this IServiceRegistry serviceRegistry) where T : BaseViewModel
        {
            return serviceRegistry.AddTransient<T>();
        }
    }
}

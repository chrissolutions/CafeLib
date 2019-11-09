using CafeLib.Core.Eventing;

namespace CafeLib.Core.IoC
{
    public static class ServiceRegistryExtensions
    {
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
        /// Add the property service to the service registry.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <returns></returns>
        public static IServiceRegistry AddPropertyService(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.AddSingleton<IPropertyService, PropertyService>();
            return serviceRegistry;
        }
    }
}

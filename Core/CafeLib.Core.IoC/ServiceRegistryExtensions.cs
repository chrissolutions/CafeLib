namespace CafeLib.Core.IoC
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <returns></returns>
        public static IServiceRegistry AddEventService(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.AddSingleton<IEventService, EventService>();
            return serviceRegistry;
        }

        /// <summary>
        /// 
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

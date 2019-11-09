namespace CafeLib.Core.IoC
{
    public static class IocFactory
    {
        /// <summary>
        /// Create the IoC service registry.
        /// </summary>
        /// <returns>service registry interface</returns>
        public static IServiceRegistry CreateRegistry()
        {
            return new ServiceRegistry();
        }
    }
}

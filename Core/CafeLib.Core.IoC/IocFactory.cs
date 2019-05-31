namespace CafeLib.Core.IoC
{
    public static class IocFactory
    {
        public static IServiceRegistry CreateRegistry()
        {
            var registry = new ServiceRegistry();
            registry.AddSingleton<IServiceResolver>(x => registry);
            return registry;
        }
    }
}

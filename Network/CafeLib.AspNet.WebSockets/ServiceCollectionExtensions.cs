using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.WebSockets
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocketServices(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IWebSocketSender, WebSocketSender>();
            return services;
        }
    }
}

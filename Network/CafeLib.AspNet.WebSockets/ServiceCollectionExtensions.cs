using System.Reflection;
using CafeLib.AspNet.WebSockets.Internal;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.WebSockets
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<IWebSocketConnectionManager, ConnectionManager>();
            services.AddSingleton<IWebSocketSender, WebSocketSender>();

            Assembly.GetEntryAssembly()?.ExportedTypes
                .Inherits<WebSocketHandler>()
                .ForEach(x => services.AddSingleton(x));

            return services;
        }
    }
}
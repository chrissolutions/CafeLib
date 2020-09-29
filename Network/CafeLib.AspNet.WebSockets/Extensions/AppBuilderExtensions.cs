using System;
using CafeLib.AspNet.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseWebSocketService<T>(this IApplicationBuilder app, PathString path) where T : IWebSocketHandler, new()
        {
            //var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            //var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            var serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            return app.Map(path, x => x.UseMiddleware<WebSocketMiddleware<T>>(serviceProvider));
        }

        public static IApplicationBuilder UseWebSocketService<T>(this IApplicationBuilder app, PathString path, T handler) where T : IWebSocketHandler, new()
        {
            //var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            //var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            var serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            return app.Map(path, x => x.UseMiddleware<WebSocketMiddleware<T>>(handler, serviceProvider));
        }
    }
}

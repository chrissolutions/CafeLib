using CafeLib.AspNet.WebSockets.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CafeLib.AspNet.WebSockets
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder MapWebSocket<T>(this IApplicationBuilder app, PathString path) where T : IWebSocketHandler
        {
            return app.Map(path, x => x.UseMiddleware<WebSocketMiddleware<T>>());
        }
    }
}
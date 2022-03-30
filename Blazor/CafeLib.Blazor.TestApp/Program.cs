using System;
using System.Net.Http;
using System.Threading.Tasks;
using CafeLib.Blazor.TestApp.Interop;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.TestApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(sp => new QrCodeProxy(sp.GetService<IJSRuntime>()));

            await builder.Build().RunAsync();
        }
    }
}

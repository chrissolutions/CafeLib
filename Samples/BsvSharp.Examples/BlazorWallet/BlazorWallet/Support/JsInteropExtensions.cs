using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;
using Microsoft.JSInterop;

namespace BlazorWallet.Support
{
    public static class JsInteropExtensions
    {
        public static async ValueTask<T> Import<T>(this IJSRuntime jsRuntime, string scriptFile) where T : IJSObjectReference
        {
            return await jsRuntime.InvokeAsync<T>("import", scriptFile);
        }

        public static async ValueTask ConsoleLog(this IJSRuntime jsRuntime, string message)
        {
            await jsRuntime.InvokeVoidAsync("console.log", message);
        }

        public static async ValueTask<bool> Confirm(this IJSRuntime jsRuntime, string message)
        {
            return await jsRuntime.InvokeAsync<bool>("confirm", message);
        }

        public static async ValueTask Alert(this IJSRuntime jsRuntime, string message)
        {
            await jsRuntime.InvokeVoidAsync("alert", message);
        }

        public static T CreateObject<T>(this IJSObjectReference jsRef, params object[] args)
        {
            var parameters = new object[] { jsRef }.Concat(args).ToArray();
            return typeof(T).CreateInstance<T>(parameters);
        }
    }
}
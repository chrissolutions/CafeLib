using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.Interop
{
    public static class JsInteropExtensions
    {
        /// <summary>
        /// Import JavaScript object
        /// </summary>
        /// <typeparam name="T">IJSObjectReference type</typeparam>
        /// <param name="jsRuntime">JavaScript runtime</param>
        /// <param name="scriptFile">script file</param>
        /// <returns>value task of JavaScript object reference</returns>
        public static ValueTask<T> Import<T>(this IJSRuntime jsRuntime, string scriptFile) where T : IJSObjectReference =>
            jsRuntime.InvokeAsync<T>("import", scriptFile);

        /// <summary>
        /// Log to console.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime</param>
        /// <param name="message">message to log to console</param>
        /// <returns>value task</returns>
        public static ValueTask ConsoleLog(this IJSRuntime jsRuntime, string message) =>
            jsRuntime.InvokeVoidAsync("console.log", message);

        /// <summary>
        /// Display standard browser confirm dialog box.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime</param>
        /// <param name="message">confirmation message</param>
        /// <returns>value task</returns>
        public static ValueTask<bool> Confirm(this IJSRuntime jsRuntime, string message) =>
            jsRuntime.InvokeAsync<bool>("confirm", message);

        /// <summary>
        /// Display standard browser alert dialog box.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime</param>
        /// <param name="message">alert message</param>
        /// <returns>value task</returns>
        public static ValueTask Alert(this IJSRuntime jsRuntime, string message) =>
            jsRuntime.InvokeVoidAsync("alert", message);

        /// <summary>
        /// Create C# object from JavaScript object reference.
        /// </summary>
        /// <typeparam name="T">C# object type</typeparam>
        /// <param name="jsRef">JavaScript object reference</param>
        /// <param name="args">object construction arguments</param>
        /// <returns>C# object</returns>
        public static T CreateObject<T>(this IJSObjectReference jsRef, params object[] args)
        {
            var parameters = new object[] { jsRef }.Concat(args).ToArray();
            return Creator.CreateInstance<T>(parameters);
        }
    }
}
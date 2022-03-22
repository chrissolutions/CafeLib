using System;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.Interop
{
    public class JsInteropProxy<T> where T : JsInteropObject
    {
        private readonly LazyAsync<IJSObjectReference> _proxy;
        private readonly string _createMethod;

        /// <summary>
        /// Blazor javascript proxy constructor.
        /// </summary>
        /// <param name="proxyScript">script file path</param>
        /// <param name="jsRuntime">javascript runtime context</param>
        public JsInteropProxy(string proxyScript, IJSRuntime jsRuntime)
            : this(proxyScript, null, jsRuntime)
        {
        }

        /// <summary>
        /// Blazor javascript proxy constructor.
        /// </summary>
        /// <param name="proxyScript">script file path</param>
        /// <param name="createMethod">proxy script creation method name</param>
        /// <param name="jsRuntime">javascript runtime context</param>
        /// <exception cref="ArgumentNullException">thrown when no creation method name is not supplied</exception>
        public JsInteropProxy(string proxyScript, string createMethod, IJSRuntime jsRuntime)
        {
            proxyScript = !string.IsNullOrWhiteSpace(proxyScript) ? proxyScript : throw new ArgumentNullException(nameof(proxyScript));
            _proxy = new LazyAsync<IJSObjectReference>(async () => await jsRuntime.Import<IJSObjectReference>(proxyScript));
            _createMethod = !string.IsNullOrWhiteSpace(createMethod) ? createMethod : "create";
        }

        /// <summary>
        /// Create javascript reference for an element.
        /// </summary>
        /// <param name="elementId">element identifier</param>
        /// <returns>javascript reference</returns>
        public async Task<IJSObjectReference> CreateElementReference(string elementId) =>
            await CreateJsReference(elementId);

        /// <summary>
        /// Create javascript reference from a list of arguments
        /// </summary>
        /// <param name="args">argument list</param>
        /// <returns>javascript reference</returns>
        public async Task<IJSObjectReference> CreateJsReference(params object[] args)
        {
            return await (await _proxy).InvokeAsync<IJSObjectReference>(_createMethod, args);
        }

        /// <summary>
        /// Create a javascript interop object from proxy.
        /// </summary>
        /// <param name="elementId">element identifier</param>
        /// <param name="args">creation arguments</param>
        /// <returns>javascript interop object</returns>
        public async Task<T> CreateObject(string elementId, params object[] args)
        {
            return (await CreateElementReference(elementId)).CreateObject<T>(args);
        }
    }
}
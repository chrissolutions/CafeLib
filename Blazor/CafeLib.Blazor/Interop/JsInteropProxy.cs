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

        public JsInteropProxy(string proxyScript, IJSRuntime jsRuntime)
            : this(proxyScript, null, jsRuntime)
        {
        }

        public JsInteropProxy(string factoryScript, string createMethod, IJSRuntime jsRuntime)
        {
            factoryScript = !string.IsNullOrWhiteSpace(factoryScript) ? factoryScript : throw new ArgumentNullException(nameof(factoryScript));
            _proxy = new LazyAsync<IJSObjectReference>(async () => await jsRuntime.Import<IJSObjectReference>(factoryScript));
            _createMethod = !string.IsNullOrWhiteSpace(createMethod) ? createMethod : "create";
        }

        public async Task<IJSObjectReference> CreateElementReference(string elementId) =>
            await CreateJsReference(elementId);

        public async Task<IJSObjectReference> CreateJsReference(params object[] args)
        {
            return await (await _proxy).InvokeAsync<IJSObjectReference>(_createMethod, args);
        }

        public async Task<T> CreateObject(string elementId, params object[] args)
        {
            return (await CreateElementReference(elementId)).CreateObject<T>(args);
        }
    }
}
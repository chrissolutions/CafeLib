using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.Interop
{
    public class JsInteropObject
    {
        protected IJSObjectReference JsInstance { get; }

        protected JsInteropObject(IJSObjectReference jsInstance)
        {
            JsInstance = jsInstance ?? throw new ArgumentNullException(nameof(jsInstance));
        }
    }
}
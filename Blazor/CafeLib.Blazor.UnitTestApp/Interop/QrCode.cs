using System.Threading.Tasks;
using CafeLib.Blazor.Interop;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.UnitTestApp.Interop
{
    public class QrCode : JsInteropObject
    {
        private QrCode(IJSObjectReference jsInstance)
            : base(jsInstance)
        {
        }

        public async Task Generate(string text)
        {
            await Instance.InvokeVoidAsync("makeCode", text);
        }
    }
}
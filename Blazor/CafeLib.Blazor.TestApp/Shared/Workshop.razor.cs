using System.Threading.Tasks;
using CafeLib.Blazor.TestApp.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.TestApp.Shared
{
    public partial class Workshop : ComponentBase
    {
        [Inject] private QrCodeProxy QrCodeProxy { get; set; }
        [Inject] private IJSRuntime JsRuntime { get; set; }

        private QrCode _qrCode;

        private string _qrText = "";

        private string _displayQrText = "";

        private async Task GenerateQrText()
        {
            _qrText = GetQrText();
            await _qrCode.Generate(_qrText);
        }

        private async Task CopyQrText()
        {
            if (!string.IsNullOrWhiteSpace(_qrText))
            {
                await CopyText("qrText", "Text Copied!");
            }
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _qrCode = await QrCodeProxy.CreateObject("qrcode");
            }
        }

        #region Helpers

        private async ValueTask CopyText(string elementId, string message)
        {
            await JsRuntime.InvokeVoidAsync("copyText", elementId);
            await JsRuntime.InvokeVoidAsync("sweetAlert", message);
        }

        private static string GetQrText()
        {
            return "This is some text use to generate a QR code.";
        }

        #endregion
    }
}

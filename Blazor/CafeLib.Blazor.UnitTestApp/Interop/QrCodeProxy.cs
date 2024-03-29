﻿using CafeLib.Blazor.Interop;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.UnitTestApp.Interop
{
    public class QrCodeProxy : JsInteropProxy<QrCode>
    {
        public QrCodeProxy(IJSRuntime jsRuntime)
            : base("./js/QrCodeProxy.js", jsRuntime)
        {
        }
    }
}
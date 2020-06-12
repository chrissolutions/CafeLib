using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CafeLib.Mobile.iOS.Renderers;
using Foundation;
using UIKit;

namespace CafeLib.Mobile.iOS
{
    public class Plugin
    {
        public static void Init()
        {
            CafeContentPageRenderer.Initialize();
            CafeContentViewRenderer.Initialize();
            //NavigationPageRenderer.Initialize();
        }
    }
}
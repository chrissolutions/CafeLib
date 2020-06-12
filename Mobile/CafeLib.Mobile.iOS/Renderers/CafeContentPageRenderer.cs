using CafeLib.Mobile.iOS.Renderers;
using CafeLib.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CafeContentPage), typeof(CafeContentPageRenderer))]

namespace CafeLib.Mobile.iOS.Renderers
{
    public class CafeContentPageRenderer : PageRenderer
    {
        private bool _isDisposed;

        internal static void Initialize()
        {
        }

        public override void ViewDidLoad()
        {
            (Element as CafeContentPage)?.Loaded();
            base.ViewDidLoad();
        }

        public override void ViewDidUnload()
        {
            if (IsViewLoaded) (Element as CafeContentPage)?.Unloaded();
            base.ViewDidUnload();
        }

        public override void DidReceiveMemoryWarning()
        {
            if (IsViewLoaded) (Element as CafeContentPage)?.Unloaded();
            base.DidReceiveMemoryWarning();
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (IsViewLoaded) (Element as CafeContentPage)?.Unloaded();
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
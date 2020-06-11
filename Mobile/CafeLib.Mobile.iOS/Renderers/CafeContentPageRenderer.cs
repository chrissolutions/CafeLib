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

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && e.NewElement != null)
            {
                (Element as CafeContentPage)?.Loaded();
            }
            else if (e.OldElement != null && e.NewElement == null)
            {
                (Element as CafeContentPage)?.Unloaded();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                (Element as CafeContentPage)?.Unloaded();
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
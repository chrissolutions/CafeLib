using CafeLib.Mobile.iOS.Renderers;
using CafeLib.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CafeContentView), typeof(CafeContentViewRenderer))]

namespace CafeLib.Mobile.iOS.Renderers
{
    public class CafeContentViewRenderer : ViewRenderer
    {
        private bool _isDisposed;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                (Element as CafeContentView)?.Loaded();
            }

            if (e.OldElement != null)
            {
                (Element as CafeContentView)?.Unloaded();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                (Element as CafeContentView)?.Unloaded();
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
using CafeLib.Mobile.iOS.Renderers;
using CafeLib.Mobile.Views;
using UIKit;
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

            if (e.OldElement == null && e.NewElement != null)
            {
                (Element as CafeContentView)?.Loaded();
            }
            else if (e.OldElement != null && e.NewElement == null)
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
using Android.Content;
using CafeLib.Mobile.Android.Renderers;
using CafeLib.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CafeContentPage), typeof(CafeContentPageRenderer))]

namespace CafeLib.Mobile.Android.Renderers
{
    public class CafeContentPageRenderer : PageRenderer
    {
        private bool _isDisposed;

        public CafeContentPageRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnAttachedToWindow()
        {
            (Element as CafeContentPage)?.Loaded();
            base.OnAttachedToWindow();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            (Element as CafeContentPage)?.Unloaded();
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
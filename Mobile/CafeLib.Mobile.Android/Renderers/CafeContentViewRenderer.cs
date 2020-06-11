using Android.Content;
using CafeLib.Mobile.Android.Renderers;
using CafeLib.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CafeContentView), typeof(CafeContentViewRenderer))]

namespace CafeLib.Mobile.Android.Renderers
{
    public class CafeContentViewRenderer : ViewRenderer
    {
        private bool _isDisposed;

        public CafeContentViewRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnAttachedToWindow()
        {
            (Element as CafeContentView)?.Loaded();
            base.OnAttachedToWindow();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            (Element as CafeContentView)?.Unloaded();
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
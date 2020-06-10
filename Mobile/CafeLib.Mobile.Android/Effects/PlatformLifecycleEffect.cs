using System.Linq;
using CafeLib.Mobile.Android.Effects;
using CafeLib.Mobile.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ResolutionGroupName(ViewLifecycleEffect.EffectGroupName)]
[assembly: ExportEffect(typeof(PlatformLifecycleEffect), ViewLifecycleEffect.EffectName)]
namespace CafeLib.Mobile.Android.Effects
{
    public class PlatformLifecycleEffect : PlatformEffect
    {
        private View _nativeView;
        private ViewLifecycleEffect _viewLifecycleEffect;

        protected override void OnAttached()
        {
            _viewLifecycleEffect = Element.Effects.OfType<ViewLifecycleEffect>().FirstOrDefault();

            _nativeView = Control ?? Container;
            _nativeView.ViewAttachedToWindow += OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow += OnViewDetachedFromWindow;
        }

        protected override void OnDetached()
        {
            var _ = Control ?? Container;
            _nativeView.ViewAttachedToWindow -= OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow -= OnViewDetachedFromWindow;
        }

        private void OnViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e) => _viewLifecycleEffect?.OnLoad(Element);

        private void OnViewDetachedFromWindow(object sender, View.ViewDetachedFromWindowEventArgs e) => _viewLifecycleEffect?.OnUnload(Element);
    }
}
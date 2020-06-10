using System;
using System.ComponentModel;
using System.Linq;
using CafeLib.Mobile.Effects;
using CafeLib.Mobile.iOS.Effects;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(ViewLifecycleEffect.EffectGroupName)]
[assembly: ExportEffect(typeof(PlatformLifecycleEffect), ViewLifecycleEffect.EffectName)]
namespace CafeLib.Mobile.iOS.Effects
{
    public class PlatformLifecycleEffect : PlatformEffect
    {
        private const NSKeyValueObservingOptions ObservingOptions = NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.OldNew | NSKeyValueObservingOptions.Prior;

        private ViewLifecycleEffect _viewLifecycleEffect;
        private IDisposable _isLoadedObserverDisposable;

        protected override void OnAttached()
        {
            _viewLifecycleEffect = Element.Effects.OfType<ViewLifecycleEffect>().FirstOrDefault();

            var nativeView = Control ?? Container;
            _isLoadedObserverDisposable = nativeView?.AddObserver("superview", ObservingOptions, IsViewLoadedObserver);
        }

        protected override void OnDetached()
        {
            _viewLifecycleEffect.OnUnload(Element);
            _isLoadedObserverDisposable.Dispose();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
        }

        private void IsViewLoadedObserver(NSObservedChange nsObservedChange)
        {
            if (nsObservedChange?.OldValue == null) return;

            if (!nsObservedChange.NewValue.Equals(NSNull.Null))
            {
                _viewLifecycleEffect?.OnLoad(Element);
            }
            else if (!nsObservedChange.OldValue.Equals(NSNull.Null))
            {
                _viewLifecycleEffect?.OnUnload(Element);
            }
        }
    }
}
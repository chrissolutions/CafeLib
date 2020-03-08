using System;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Effects
{
    public class ViewLifecycleEffect : RoutingEffect
    {
        public const string EffectGroupName = "XFLifecycle";
        public const string EffectName = "ViewLifecycleEffect";

        public event EventHandler<EventArgs> Loaded;
        public event EventHandler<EventArgs> Unloaded;

        public ViewLifecycleEffect()
            : base($"{EffectGroupName}.{EffectName}")
        {
        }

        public void OnLoad(Element element) => Loaded?.Invoke(element, EventArgs.Empty);

        public void OnUnload(Element element) => Unloaded?.Invoke(element, EventArgs.Empty);
    }
}

using Xamarin.Forms;
using Xamarin.Forms.Internals;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.UnitTest.Core.Fakes
{
    public class FakeDeviceInfo : DeviceInfo
    {
        public FakeDeviceInfo()
        {
            CurrentOrientation = DeviceOrientation.Portrait;
            PixelScreenSize = new Size(360.0, 760.0);
            ScalingFactor = 1.0;
        }

        public override Size PixelScreenSize { get; }
        public override Size ScaledScreenSize => new Size(PixelScreenSize.Width / ScalingFactor, PixelScreenSize.Height / ScalingFactor);
        public override double ScalingFactor { get; }
    }
}

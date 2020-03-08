using Xamarin.Forms.Internals;

namespace CafeLib.Mobile.UnitTest.Core.Fakes
{
    internal class FakeTicker : Ticker
    {
        private bool _enabled;

        protected override void EnableTimer()
        {
            _enabled = true;

            while (_enabled)
            {
                SendSignals(16);
            }
        }

        protected override void DisableTimer()
        {
            _enabled = false;
        }
    }
}
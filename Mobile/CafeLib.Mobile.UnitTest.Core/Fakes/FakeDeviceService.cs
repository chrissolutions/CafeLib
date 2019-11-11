using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Mobile.Services;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.UnitTest.Core.Fakes
{
    internal class FakeDeviceService : IDeviceService
    {
        public bool IsOnMainThread()
        {
            return true;
        }

        public void RunOnMainThread(Action action)
        {
            if (IsOnMainThread())
            {
                action();
            }
            else
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(action);
            }
        }

        public void RunOnWorkerThread(Action action, CancellationToken cancellationToken = default)
        {
            Task.Run(action, cancellationToken);
        }

        public void QueueOnMainThread(Action action)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(action);
        }
    }
}
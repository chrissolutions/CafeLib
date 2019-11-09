using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Mobile.Services;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Test.Core.Fakes
{
    internal class FakeDeviceService : IDeviceService
    {
        public bool IsOnMainThread()
        {
            return true;
        }

        public void RunOnMainThread(Action action)
        {
            action();
        }

        public void QueueOnMainThread(Action action)
        {
            Thread.Sleep(1000);
            action();
        }

        public void RunOnWorkerThread(Action action, CancellationToken cancellationToken = default)
        {
            Task.Run(action, cancellationToken);
        }
    }
}
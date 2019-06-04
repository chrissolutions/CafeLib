using System;
using JetBrains.Annotations;

namespace CafeLib.Mobile.Core.Services
{
    public interface IDeviceService
    {
        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        [UsedImplicitly]
        void RunOnMainThread(Action action);
    }
}

using System;
using System.Threading;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Services
{
    public interface IDeviceService
    {
        /// <summary>
        /// Check whether the current managed thread id is the main thread.
        /// </summary>
        /// <returns>true if running on the main thread; otherwise false.</returns>
        bool IsOnMainThread();

        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        void RunOnMainThread(Action action);

        /// <summary>
        /// Schedules an action on the main thread.
        /// </summary>
        /// <param name="action"></param>
        void QueueOnMainThread(Action action);

        /// <summary>
        /// Runs an action on a background worker thread.
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="cancellationToken"></param>
        void RunOnWorkerThread(Action action, CancellationToken cancellationToken = default);
    }
}

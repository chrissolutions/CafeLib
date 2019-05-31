using System;
using System.Threading.Tasks;

namespace CafeLib.Mobile.Core.Services
{
    public interface IDeviceService : IServiceProvider
    {
        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        void RunOnMainThread(Action action);
    }
}

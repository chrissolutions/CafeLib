using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Runnable
{
    /// <summary>
    /// Runner interface.
    /// </summary>
    public interface IRunnable : IDisposable
    {
        Task Start();
        Task Stop();
        bool IsRunning { get; }
    }
}

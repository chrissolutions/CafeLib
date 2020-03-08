using System;
using System.Threading.Tasks;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

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
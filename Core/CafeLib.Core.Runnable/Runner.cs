using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Diagnostics;
using CafeLib.Core.Eventing;

namespace CafeLib.Core.Runnable
{
    /// <summary>
    /// Runs a background task.
    /// </summary>
    public abstract class Runner : IRunnable
    {
        #region Private Variables

        private static readonly object Mutex = new object();
        private int _delay;
        private bool _disposed;
        private Action<IEventMessage> _runnerEvent = delegate { };

        #endregion

        #region Properties

        /// <summary>
        /// Runner name.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// Log event handler.
        /// </summary>
        protected Action<IEventMessage> RunnerEvent
        {
            get => _runnerEvent;
            set => _runnerEvent = value ?? delegate { };
        }

        /// <summary>
        /// Runner delay duration in milliseconds.
        /// </summary>
        protected int Delay
        {
            get => _delay;
            set => _delay = value > 0 ? value : 0;
        }

        /// <summary>
        /// Cancellation source.
        /// </summary>
        protected CancellationTokenSource CancellationSource { get; set; }

        /// <summary>
        /// Determines whether the service is running.
        /// </summary>
        public bool IsRunning => CancellationSource != null && !CancellationSource.IsCancellationRequested;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceRunnerBase constructor.
        /// </summary>
        /// <param name="delay">runner delay duration</param>
        protected Runner(uint delay = 0)
        {
            Name = GetType().Name;
            CancellationSource = null;
            Delay = (int)delay;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the service.
        /// </summary>
        public virtual async Task Start()
        {
            if (!IsRunning)
            {
                lock (Mutex)
                {
                    CancellationSource = new CancellationTokenSource();
                    RunnerEvent.Invoke(new RunnerEventMessage(ErrorLevel.Ignore, $"{Name} started."));
                    RunTask();
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        public virtual async Task Stop()
        {
            if (IsRunning)
            {
                lock (Mutex)
                {
                    CancellationSource.Cancel();
                    RunnerEvent?.Invoke(new RunnerEventMessage(ErrorLevel.Ignore, $"{Name} stopped."));
                }
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Run the service.
        /// </summary>
        protected abstract Task Run();

        /// <summary>
        /// Run loop.
        /// </summary>
        /// <returns>awaitable task</returns>
        protected async Task RunLoop()
        {
            while (IsRunning)
            {
                try
                {
                    await Run();
                }
                catch (Exception ex)
                {
                    RunnerEvent.Invoke(new RunnerEventMessage(ErrorLevel.Error, $"{Name} exception: {ex.Message} {ex.InnerException?.Message}", ex));
                }

                await Task.Delay(Delay, CancellationSource.Token);
            }
        }

        /// <summary>
        /// Run the task in the background.
        /// </summary>
        protected void RunTask()
        {
            Task.Run(async () =>
            {
                try
                {
                    await RunLoop();
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    ExitTask();
                }

            }, CancellationSource.Token);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Exit the background task.
        /// </summary>
        private void ExitTask()
        {
            lock (Mutex)
            {
                CancellationSource?.Dispose();
                CancellationSource = null;
            }
        }

        #endregion

        #region IDisposible

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose concurrent queue.
        /// </summary>
        /// <param name="disposing">indicate whether the queue is disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            try
            {
                CancellationSource?.Dispose();
            }
            catch
            {
                // ignore
            }
            finally
            {
                CancellationSource = null;
            }
        }

        #endregion
    }
}

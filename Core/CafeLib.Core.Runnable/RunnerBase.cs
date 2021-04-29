using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Eventing;
using CafeLib.Core.Runnable.Events;

namespace CafeLib.Core.Runnable
{
    /// <summary>
    /// Runs a background task.
    /// </summary>
    public abstract class RunnerBase : IRunnable
    {
        #region Private Variables

        private int _delay;
        private bool _disposed;
        private CancellationTokenSource _cancellationSource;

        #endregion

        #region Events

        protected event Action<IEventMessage> Advised = x => { };

        #endregion

        #region Properties

        /// <summary>
        /// Runner name.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// Runner delay duration in milliseconds.
        /// </summary>
        protected int Delay
        {
            get => _delay;
            set => _delay = value > 0 ? value : 0;
        }

        /// <summary>
        /// Determines whether the service is running.
        /// </summary>
        public bool IsRunning => !_cancellationSource?.IsCancellationRequested ?? false;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceRunnerBase constructor.
        /// </summary>
        /// <param name="delay">runner delay duration</param>
        protected RunnerBase(int delay = default)
        {
            Name = GetType().Name;
            _cancellationSource = null;
            Delay = delay;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the runner.
        /// </summary>
        /// <returns>start task</returns>
        public virtual Task Start()
        {
            if (!IsRunning)
            {
                _cancellationSource = new CancellationTokenSource();
                OnAdvise(new RunnerStartMessage($"{Name} started."));
                RunLoop();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        public virtual Task Stop()
        {
            if (IsRunning)
            {
                Cancel();
                OnAdvise(new RunnerStopMessage($"{Name} stopped."));
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Run the service.
        /// </summary>
        protected abstract Task Run();

        /// <summary>
        /// Raise advise event.
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnAdvise(IEventMessage message)
        {
            Advised.Invoke(message);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Run the loop in the background.
        /// </summary>
        private void RunLoop()
        {
            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    try
                    {
                        await Run().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        OnAdvise(new RunnerEventMessage(ex.Message, ex));
                    }

                    await Task.Delay(Delay, _cancellationSource?.Token ?? default).ConfigureAwait(false);
                }

            }, _cancellationSource.Token);
        }

        private void Cancel()
        {
            _cancellationSource.Cancel();
            _cancellationSource.Dispose();
            _cancellationSource = null;
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
                _cancellationSource?.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        #endregion
    }
}

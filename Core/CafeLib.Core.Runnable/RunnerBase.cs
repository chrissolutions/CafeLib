using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Eventing;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Runnable
{
    /// <summary>
    /// Runs a background task.
    /// </summary>
    public abstract class RunnerBase : IRunnable
    {
        #region Private Variables

        private readonly AsyncLocal<int> _delay;
        private readonly AsyncLocal<bool> _disposed;
        private readonly AsyncLocal<CancellationTokenSource> _cancellationSource;
        private readonly AsyncLocal<string> _name;

        #endregion

        #region Constructors

        /// <summary>
        /// RunnerBase default constructor.
        /// </summary>
        protected RunnerBase()
        {
            _delay = new AsyncLocal<int> { Value = 0 };
            _disposed = new AsyncLocal<bool> { Value = false };
            _cancellationSource = new AsyncLocal<CancellationTokenSource> { Value = null };
            _name = new AsyncLocal<string> { Value = GetType().Name };
        }

        /// <summary>
        /// ServiceRunnerBase constructor.
        /// </summary>
        /// <param name="delay">runner delay duration</param>
        protected RunnerBase(int delay)
            : this(null, delay)
        {
        }

        /// <summary>
        /// RunnerBase constructor.
        /// </summary>
        /// <param name="name">name of runner base</param>
        /// <param name="delay">runner delay duration</param>
        protected RunnerBase(string name, int delay)
            : this()
        {
            Name = name;
            Delay = delay;
        }

        #endregion

        #region Events

        protected event Action<IEventMessage> Advised = x => { };

        #endregion

        #region Properties

        /// <summary>
        /// Disposed flag.
        /// </summary>
        private bool Disposed
        {
            get => _disposed.Value;
            set => _disposed.Value = value;
        }

        /// <summary>
        /// CancellationSource
        /// </summary>
        private CancellationTokenSource CancellationSource
        {
            get => _cancellationSource.Value;
            set => _cancellationSource.Value = value;
        }

        /// <summary>
        /// Runner cancellation token.
        /// </summary>
        protected CancellationToken CancellationToken => CancellationSource.Token;

        /// <summary>
        /// Runner name.
        /// </summary>
        protected string Name
        {
            get => _name.Value;
            set => _name.Value = !string.IsNullOrWhiteSpace(value) ? value : _name.Value;
        }

        /// <summary>
        /// Runner delay duration in milliseconds.
        /// </summary>
        protected int Delay
        {
            get => _delay.Value;
            set => _delay.Value = value > 0 ? value : 0;
        }

        /// <summary>
        /// Determines whether the service is running.
        /// </summary>
        public bool IsRunning => !_cancellationSource.Value?.IsCancellationRequested ?? false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <returns>start task</returns>
        public virtual Task Start()
        {
            if (!IsRunning)
            {
                CancellationSource = new CancellationTokenSource();
                OnAdvise(new RunnerEventMessage($"{Name} started."));
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
                CancellationSource.Cancel();
                OnAdvise(new RunnerEventMessage($"{Name} stopped."));
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
                        await Run();
                    }
                    catch (Exception ex)
                    {
                        OnAdvise(new RunnerEventMessage($"{Name} exception: {ex.Message} {ex.InnerException?.Message}"));
                    }

                    await Task.Delay(Delay, CancellationSource.Token);
                }

            }, CancellationSource.Token);
        }

        #endregion

        #region IDisposible

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(!Disposed);
            Disposed = true;
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

            CancellationSource = null;
            Delay = 0;
        }

        #endregion
    }
}

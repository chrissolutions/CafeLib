﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Eventing;
using CafeLib.Core.Support;

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

        public event Action<IEventMessage> Advised = x => { };

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
            set => _delay = value > 0 ? value : default;
        }

        /// <summary>
        /// Determines whether the service is running.
        /// </summary>
        public bool IsRunning => _cancellationSource != null && !_cancellationSource.IsCancellationRequested;

        #endregion

        #region Constructors

        /// <summary>
        /// ServiceRunnerBase constructor.
        /// </summary>
        /// <param name="delay">runner delay duration</param>
        protected RunnerBase(int delay)
            : this((uint)delay)
        {
        }

        /// <summary>
        /// ServiceRunnerBase constructor.
        /// </summary>
        /// <param name="delay">runner delay duration</param>
        protected RunnerBase(uint delay = default)
        {
            Name = GetType().Name;
            _cancellationSource = null;
            Delay = (int)delay;
        }

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
                _cancellationSource = new CancellationTokenSource();
                OnAdvise(new RunnerEventMessage(ErrorLevel.Ignore, $"{Name} started."));
                BeginLoop();
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
                _cancellationSource.Cancel();
                OnAdvise(new RunnerEventMessage(ErrorLevel.Ignore, $"{Name} stopped."));
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
        /// Begin the loop in the background.
        /// </summary>
        private void BeginLoop()
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
                    ExitLoop();
                }

            }, _cancellationSource.Token);
        }

        /// <summary>
        /// Run loop.
        /// </summary>
        /// <returns>awaitable task</returns>
        private async Task RunLoop()
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

                await Task.Delay(Delay, _cancellationSource.Token);
            }
        }

        /// <summary>
        /// Exit the background loop.
        /// </summary>
        private void ExitLoop()
        {
            _cancellationSource?.Dispose();
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
            finally
            {
                _cancellationSource = null;
            }
        }

        #endregion
    }
}

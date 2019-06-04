using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.IoC;
using CafeLib.Core.Runnable;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.ViewModels
{ 
    [UsedImplicitly]
    public abstract class RunnableViewModel<T> : BaseViewModel<T>, IRunnable where T : Page
    {
        private CancellationTokenSource _cancellationSource;
        private bool _disposed;

        /// <summary>
        /// BaseViewModel constructor.
        /// </summary>
        /// <param name="resolver"></param>
        protected RunnableViewModel(IServiceResolver resolver)
            : base(resolver)
        {
        }

        public bool IsRunning => _cancellationSource.IsCancellationRequested;

        public CancellationToken CancellationToken { get; protected set; }

        public virtual async Task Start()
        {
            _cancellationSource = new CancellationTokenSource();
            await Task.FromResult(0);
        }

        public virtual async Task Stop()
        {
            _cancellationSource.Cancel();
            await Task.FromResult(0);
        }

        [UsedImplicitly]
        public async void Delay(int milliseconds)
        {
            try
            {
                await Task.Delay(milliseconds, CancellationToken);
            }
            catch
            {
                // ignore
            }
        }

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
    }
}

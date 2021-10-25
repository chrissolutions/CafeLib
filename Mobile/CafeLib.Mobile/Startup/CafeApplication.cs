using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.Services;
using Xamarin.Forms;

namespace CafeLib.Mobile.Startup
{
    /// <summary>
    /// Base class of CafeLib mobile application.
    /// </summary>
    [SuppressMessage("ReSharper", "AsyncVoidLambda")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class CafeApplication : Application, IAlertService, IDisposable
    {
        protected IServiceRegistry Registry { get; }
        public IServiceResolver Resolver => Registry.GetResolver();

        /// <summary>
        /// Default constructor used to suppress XAML warnings.
        /// </summary>
        // ReSharper disable once PublicConstructorInAbstractClass
        public CafeApplication()
        {
            throw new Exception(@"Cannot instantiate cafe application via default constructor.");
        }

        /// <summary>
        /// Cafe mobile application constructor.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        protected CafeApplication(IServiceRegistry serviceRegistry)
        {
            Registry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
        }

        /// <summary>
        /// Configure the application service registry.
        /// </summary>
        public virtual void Configure()
        {
        }

        /// <summary>
        /// Dispose application.
        /// </summary>
        public void Dispose()
        {
            Resolver.Dispose();
        }

        /// <summary>
        /// Display an alert dialog.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="ok">accept button display</param>
        public async Task DisplayAlert(string title, string message, string ok = "OK")
        {
            var completed = new TaskCompletionSource<bool>();

            Current.RunOnMainThread(async () =>
            {
                await MainPage.DisplayAlert(title, message, ok);
                completed.SetResult(true);
            });

            await completed.Task;
        }

        /// <summary>
        /// Display confirmation dialog
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="ok">accept button display</param>
        /// <param name="cancel">cancel button display</param>
        /// <returns>true for OK, false for cancel</returns>
        public async Task<bool> DisplayConfirm(string title, string message, string ok = "OK", string cancel = "Cancel")
        {
            var completed = new TaskCompletionSource<bool>();

            Current.RunOnMainThread(async () =>
            {
                var answer = await MainPage.DisplayAlert(title, message, ok, cancel);
                completed.SetResult(answer);
            });

            return await completed.Task;
        }

        /// <summary>
        /// Display option selection dialog.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="cancel">cancel button display</param>
        /// <param name="delete">delete button display</param>
        /// <param name="options">enumerable list of option strings</param>
        /// <returns>the selected option string</returns>
        public async Task<string> DisplayOptions(string title, string cancel, string delete, IEnumerable<string> options)
        {
            var completed = new TaskCompletionSource<string>();

            Current.RunOnMainThread(async () =>
            {
                var answer = await MainPage.DisplayActionSheet(title, cancel, delete, options.ToArray());
                completed.SetResult(answer);
            });

            return await completed.Task;
        }
    }
}

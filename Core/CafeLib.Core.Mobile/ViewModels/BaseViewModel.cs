using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.Mobile.Services;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.ViewModels
{
    public abstract class BaseViewModel : ObservableBase
    {
        #region Properties

        private string _title;
        public string Title
        {
            get => _title;
            set => SetValue(ref _title, value);
        }

        protected INavigationService Navigation => RegisterServices.NavigationService;

        #endregion

        #region Methods

        /// <summary>
        /// Bind view model to associated page.
        /// </summary>
        protected void Bind()
        {
            // Obtain the page.
            var page = GetPage();

            // Clear existing binding context.
            page.BindingContext = null;

            // Bind view model to page.
            page.BindingContext = this;
        }

        /// <summary>
        /// Displays an alert on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        public void DisplayAlert(string title, string message, string ok = "OK")
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, ok);
            });
        }

        /// <summary>
        /// Displays an alert (simple question) on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        /// <param name="cancel">cancel</param>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public async Task<bool> DisplayConfirm(string title, string message, string ok = "OK", string cancel = "Cancel")
        {
            var completed = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var answer = await Application.Current.MainPage.DisplayAlert(title, message, ok, cancel);
                completed.SetResult(answer);
            });

            return await completed.Task;
        }

        /// <summary>
        /// Displays an action sheet (list of buttons) on the page, asking for user input.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="cancel">cancellation string</param>
        /// <param name="destroy">destroy string</param>
        /// <param name="options">option list</param>
        /// <returns></returns>
        public async Task<string> DisplayActionSheet(string title, string cancel, string destroy, IEnumerable<string> options)
        {
            var completed = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var answer = await Application.Current.MainPage.DisplayActionSheet(title, cancel, destroy, options.ToArray());
                completed.SetResult(answer);
            });

            return await completed.Task;
        }

        /// <summary>
        /// Obtain the page associated with the view model.
        /// </summary>
        /// <returns>page</returns>
        protected Page GetPage()
        {
            return RegisterServices.PageService.ResolvePage(this);
        }

        /// <summary>
        /// Obtain assoicated page casted to the requested page type.
        /// </summary>
        /// <typeparam name="T">page type</typeparam>
        /// <returns>page</returns>
        protected T GetPage<T>() where T : Page
        {
            return (T)RegisterServices.PageService.ResolvePage(this);
        }

        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        protected virtual async Task RunOnMainThread(Action action)
        {
            var completed = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action?.Invoke();
                    completed.SetResult(true);
                }
                catch (Exception ex)
                {
                    completed.SetException(ex);
                }
            });

            await completed.Task;
        }

        #endregion
    }
}

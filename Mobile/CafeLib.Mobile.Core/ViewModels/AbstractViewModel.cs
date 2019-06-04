using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Mobile.Core.Extensions;
using CafeLib.Mobile.Core.Services;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.ViewModels
{
    public abstract class AbstractViewModel : ObservableBase
    {
        #region Constructors

        /// <summary>
        /// BaseViewModel constructor.
        /// </summary>
        protected AbstractViewModel()
        {
            Bind();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Title.
        /// </summary>
        private string _title;
        [UsedImplicitly]
        public string Title
        {
            get => _title;
            set => SetValue(ref _title, value);
        }

        /// <summary>
        /// Page service.
        /// </summary>
        protected IPageService PageService => MobileServices.PageService;

        /// <summary>
        /// Navigation service.
        /// </summary>
        protected INavigationService NavigationService => MobileServices.NavigationService;

        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        protected IDeviceService DeviceService => MobileServices.DeviceService;

        #endregion

        #region Public Methods

        /// <summary>
        /// Establish viewmodel as the navigation page.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public Page AsNavigationPage()
        {
            return NavigationService.SetNavigationPage(ResolvePage());
        }

        /// <summary>
        /// Bind view model to associated page.
        /// </summary>
        public void Bind()
        {
            // Clear existing binding context.
            ResolvePage().BindingContext = null;

            // Bind view model to page.
            ResolvePage().BindingContext = this;
        }

        /// <summary>
        /// Obtain the page associated with the view model.
        /// </summary>
        /// <returns>page</returns>
        public abstract Page ResolvePage();

        /// <summary>
        /// Displays an alert on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        [UsedImplicitly]
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
        [UsedImplicitly]
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
        [UsedImplicitly]
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
        /// Push page assoiciated with the viewmodel onto the navigation stack.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="animate"></param>
        /// <returns></returns>
        [UsedImplicitly]
        public async Task PushAsync(AbstractViewModel viewModel, bool animate = false)
        {
            viewModel.ResolvePage().SetViewModel(viewModel);
            await NavigationService.NavigationPage.Navigation.PushAsync(viewModel.ResolvePage());
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        [UsedImplicitly]
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

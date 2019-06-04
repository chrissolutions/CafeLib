using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Core.Extensions;
using CafeLib.Mobile.Core.Support;
using CafeLib.Mobile.Core.ViewModels;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.Services
{
    internal class MobileService : IPageService, INavigationService, IDeviceService
    {
        #region Private Members

        private readonly Dictionary<Type, PageResolver> _pageResolvers;

        #endregion

        #region Constructors

        /// <summary>
        /// Bootstrapper constructor
        /// </summary>
        internal MobileService()
        {
            _pageResolvers = new Dictionary<Type, PageResolver>();
        }

        #endregion

        #region Properties

        public Page NavigationPage { get; private set; }

        #endregion

        #region Methods

        [UsedImplicitly]
        public Task InsertBeforeAsync(Page page, Page currentViewModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPage1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TPage2"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="currentViewModel"></param>
        /// <returns></returns>
        public async Task InsertBeforeAsync<T1, TPage1, T2, TPage2>(T1 viewModel, T2 currentViewModel)
            where T1 : BaseViewModel<TPage1>
            where TPage1 : Page
            where T2 : BaseViewModel<TPage2>
            where TPage2 : Page
        {
            var page = ResolvePage<T1, TPage1>();
            var currentPage = ResolvePage<T2, TPage2>();
            Application.Current.MainPage.Navigation.InsertPageBefore(page, currentPage);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Resolves page associated with the viewmodel.
        /// </summary>
        /// <returns>bounded page</returns>
        [UsedImplicitly]
        public TPage ResolvePage<T, TPage>(T viewModel) where T : BaseViewModel<TPage> where TPage : Page
        {
            // Check resolver registration.
            CheckResolverRegistration<T, TPage>();

            // Obtain page from resolver.
            return GetPageFromResolver<T, TPage>();
        }

        public void DisplayAlert(string title, string message, string ok = "OK")
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisplayConfirm(string title, string message, string ok = "OK", string cancel = "Cancel")
        {
            throw new NotImplementedException();
        }

        public Task<string> DisplayPopup(string title, string cancel, string destroy, IEnumerable<string> options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves page associated with the viewmodel.
        /// </summary>
        /// <returns>bounded page</returns>
        public TPage ResolvePage<T, TPage>() where T : BaseViewModel<TPage> where TPage : Page
        {
            // Check resolver registration.
            CheckResolverRegistration<T, TPage>();

            // Obtain page from resolver.
            return GetPageFromResolver<T, TPage>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="animate"></param>
        /// <returns></returns>
        public async Task PushAsync(Page page, bool animate = false)
        {
            if (page != null)
            {
                await NavigationPage.Navigation.PushAsync(page, animate);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <param name="animate"></param>
        /// <returns></returns>
        public async Task<TPage> PopAsync<TPage>(bool animate = false) where TPage : Page
        {
            var completed = new TaskCompletionSource<TPage>();
            RunOnMainThread(async () =>
            {
                var page = await Application.Current.MainPage.Navigation.PopAsync(animate);
                completed.SetResult((TPage)page);
            });

            return await completed.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPage"></typeparam>
        /// <param name="animate"></param>
        /// <returns></returns>
        public async Task<T> PopAsync<T, TPage>(bool animate = false) where T : BaseViewModel<TPage> where TPage : Page
        {
            var page = (TPage) await Application.Current.MainPage.Navigation.PopAsync(animate);
            return page.GetViewModel<T, TPage>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public void RunOnMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

        /// <summary>
        /// Set the application navigator.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>previous navigator</returns>
        public Page SetNavigationPage(Page page)
        {
            NavigationPage = new NavigationPage(page);
            return NavigationPage;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Register view models.
        /// </summary>
        private void CheckResolverRegistration<T, TPage>() where T : BaseViewModel<TPage> where TPage : Page
        {
            if (!_pageResolvers.ContainsKey(typeof(T)))
            {
                _pageResolvers.Add(typeof(T), new PageResolver(typeof(TPage)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPage"></typeparam>
        /// <returns></returns>
        private TPage GetPageFromResolver<T, TPage>() where T : BaseViewModel<TPage> where TPage : Page
        {
            // Leave if no page resolver exist for this view model type.
            if (!_pageResolvers.ContainsKey(typeof(T))) return null;

            // Resolve the page.
            var pageResolver = _pageResolvers[typeof(T)];
            return (TPage)pageResolver.Resolve();
        }

        #endregion
    }
}

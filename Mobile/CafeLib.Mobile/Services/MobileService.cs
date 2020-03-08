using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Attributes;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.Startup;
using CafeLib.Mobile.Support;
using CafeLib.Mobile.ViewModels;
using CafeLib.Mobile.Views;
using Xamarin.Forms;

namespace CafeLib.Mobile.Services
{
    internal sealed class MobileService : IPageService, IAlertService, INavigationService, IDeviceService, IServiceResolver
    {
        private readonly Assembly _appAssembly;
        private readonly Dictionary<Type, PageResolver> _pageResolvers;
        private readonly IServiceResolver _resolver;
        private readonly int _mainThreadId;

        private const string ViewModelSuffix = "ViewModel";
        private const string PageSuffix = "Page";
        private const string DefaultAcceptText = "OK";
        private const string DefaultCancelText = "Cancel";

        /// <summary>
        /// Bootstrapper constructor
        /// </summary>
        internal MobileService(IServiceResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _appAssembly = Application.Current.GetType().Assembly;
            _pageResolvers = new Dictionary<Type, PageResolver>();
            _mainThreadId = Environment.CurrentManagedThreadId;
            InitPageResolvers();
        }

        public Page CurrentPage => GetCurrentNavigationPage().NavigationStack.LastOrDefault();

        public NavigationPage CurrentNavigation => GetCurrentPage();

        public bool IsCurrent<T>(T viewModel) where T : BaseViewModel
        {
            return CurrentPage.GetViewModel<T>() == viewModel;
        }

        /// <summary>
        /// Resolve the dependency.
        /// </summary>
        /// <typeparam name="T">dependency type</typeparam>
        /// <returns>instance of dependency type</returns>
        public T Resolve<T>() where T : class
        {
            return _resolver.Resolve<T>();
        }

        /// <summary>
        /// Resolve viewmodel type to is associated view.
        /// </summary>
        /// <typeparam name="TViewModel">view model type</typeparam>
        /// <returns>page instance that corresponds to the view model type</returns>
        public Page ResolvePage<TViewModel>() where TViewModel : BaseViewModel
        {
            return ResolvePage(typeof(TViewModel));
        }

        /// <summary>
        /// Resolve the page associated to the view model.
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>page instance that corresponds to the view model type</returns>
        public Page ResolvePage(BaseViewModel viewModel)
        {
            return ResolvePage(viewModel.GetType());
        }

        /// <summary>
        /// Display an alert dialog.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="ok">accept button display</param>
        public Task DisplayAlert(string title, string message, string ok = DefaultAcceptText)
        {
            return ((CafeApplication)Application.Current).DisplayAlert(title, message, ok);
        }

        /// <summary>
        /// Display confirmation dialog
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="ok">accept button display</param>
        /// <param name="cancel">cancel button display</param>
        /// <returns>true for OK, false for cancel</returns>
        public async Task<bool> DisplayConfirm(string title, string message, string ok = DefaultAcceptText, string cancel = DefaultCancelText)
        {
            return await ((CafeApplication)Application.Current).DisplayConfirm(title, message, ok, cancel);
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
            return await ((CafeApplication)Application.Current).DisplayOptions(title, cancel, delete, options);
        }

        /// <summary>
        /// Insert viewmodel ahead of another viewmodel
        /// </summary>
        /// <typeparam name="T1">type of view model to insert before</typeparam>
        /// <typeparam name="T2">type of the current view model</typeparam>
        /// <param name="viewModel">view model to insert before</param>
        /// <param name="currentViewModel">current view model</param>
        /// <returns>awaitable task</returns>
        public void InsertBefore<T1, T2>(T1 viewModel, T2 currentViewModel) where T1 : BaseViewModel where T2 : BaseViewModel
        {
            GetCurrentNavigationPage().InsertPageBefore(viewModel.ResolvePage(), currentViewModel.ResolvePage());
        }

        /// <summary>
        /// Asynchronously adds page to the top of the navigation stack.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">optional animation</param>
        /// <returns></returns>
        public async Task PushAsync<T>(T viewModel, bool animate = false) where T : BaseViewModel
        {
            var vm = viewModel ?? Resolve<T>();
            var page = vm.ResolvePage();

            page.SetViewModel(vm);
            await GetCurrentNavigationPage().PushAsync(page, animate);
        }

        /// <summary>
        /// Asynchronously adds page to the top of the modal stack.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">optional animation</param>
        /// <returns></returns>
        public async Task PushModalAsync<T>(T viewModel, bool animate = false) where T : BaseViewModel
        {
            var vm = viewModel ?? Resolve<T>();
            var page = vm.ResolvePage();
            page.SetViewModel(vm);

            var showModalInNav = page.GetAttribute<ShowModalInNavAttribute>();
            if (showModalInNav != null)
            {
                await GetCurrentNavigationPage().PushModalAsync(new ModalNavigationPage(page), animate);
            }
            else
            {
                await GetCurrentNavigationPage().PushModalAsync(page.HasToolbarItems() ? page.AsNavigationPage<ModalNavigationPage>() : page, animate);
            }
        }

        /// <summary>
        /// Asynchronously remove most recent page from the navigation stack.
        /// </summary>
        /// <param name="animate">optional animation</param>
        /// <returns>The page previously at top of the navigation stack</returns>
        public async Task PopAsync(bool animate = false)
        {
            var page = await GetCurrentNavigationPage().PopAsync(animate);
            if (page != null)
            {
                ReleasePage(page.BindingContext?.GetType());
                page.Parent = null;
                page.BindingContext = null;
            }
        }

        /// <summary>
        /// Asynchronously remove most recent page from the modal stack.
        /// </summary>
        /// <param name="animate">optional animation</param>
        /// <returns>The page previously at top of the navigation stack</returns>
        public async Task PopModalAsync(bool animate = false)
        {
            var navPage = GetCurrentNavigationPage();
            var page = navPage.NavigationStack.LastOrDefault();
            await navPage.PopModalAsync(animate);
            if (page != null)
            {
                ReleasePage(page.BindingContext?.GetType());
                page.Parent = null;
                page.BindingContext = null;
            }
        }

        /// <summary>
        /// Pops all but the root Page off the navigation stack.
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        public async Task PopToRootAsync(bool animate = false)
        {
            await GetCurrentNavigationPage().PopModalAsync(animate);
        }

        /// <summary>
        /// Remove from navigation stack.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        public void Remove<T>(T viewModel) where T : BaseViewModel
        {
            RunOnMainThread(() =>
            {
                var vm = viewModel ?? Resolve<T>();
                var page = vm.ResolvePage();
                GetCurrentNavigationPage().RemovePage(page);
            });
        }

        /// <summary>
        /// Check whether the current managed thread id is the main thread.
        /// </summary>
        /// <returns>true if running on the main thread; otherwise false.</returns>
        public bool IsOnMainThread() => Environment.CurrentManagedThreadId == _mainThreadId;

        /// <summary>
        /// Run action on main UI thread.
        /// </summary>
        /// <param name="action">action</param>
        public void RunOnMainThread(Action action)
        {
            if (IsOnMainThread())
            {
                action();
            }
            else
            {
                QueueOnMainThread(action);
            }
        }

        /// <summary>
        /// Schedules an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        public void QueueOnMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

        /// <summary>
        /// Runs an action on a background worker thread.
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="cancellationToken"></param>
        public void RunOnWorkerThread(Action action, CancellationToken cancellationToken = default)
        {
            if (IsOnMainThread())
            {
                Task.Run(action, cancellationToken);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _resolver?.Dispose();
        }

        ///// <summary>
        ///// Create an application navigator.
        ///// </summary>
        ///// <param name="page">content page</param>
        ///// <returns>previous navigator</returns>
        //private NavigationPage CreateNavigator(Page page)
        //{
        //    if (page == null) throw new ArgumentNullException(nameof(page));
        //    var contentPage = page is MasterDetailPage masterDetailPage ? masterDetailPage.Detail : page;
        //    return contentPage.IsNavigationPage() ? (NavigationPage)contentPage : new NavigationPage(contentPage);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static NavigationPage GetCurrentPage()
        {
            NavigationPage GetPage(Page page)
            {
                var modalNavPage = page.Navigation.ModalStack.LastOrDefault(x => x.IsNavigationPage());

                if (modalNavPage != null && page != modalNavPage)
                {
                    return GetPage(modalNavPage);
                }

                var navPage = page.Navigation.NavigationStack.LastOrDefault(x => x.IsNavigationPage());

                if (navPage != null && page != navPage)
                {
                    return GetPage(navPage);
                }

                return page as NavigationPage;
            }
            return GetPage(Application.Current.MainPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static INavigation GetCurrentNavigationPage() => GetCurrentPage().Navigation;

        /// <summary>
        /// Set up page resolvers.
        /// </summary>
        private void InitPageResolvers()
        {
            var viewModelTypeInfos = _appAssembly
                .CreatableTypes()
                .Inherits<BaseViewModel>()
                .EndsWith(ViewModelSuffix);

            foreach (var viewModelTypeInfo in viewModelTypeInfos)
            {
                var pageType = FindPageType(viewModelTypeInfo);
                if (pageType == null) continue;
                _pageResolvers.Add(viewModelTypeInfo.AsType(), new PageResolver(pageType));
            }
        }

        /// <summary>
        /// Find corresponding page type for a view model type.
        /// </summary>
        /// <param name="viewModelTypeInfo">view model type</param>
        /// <returns>page type</returns>
        private Type FindPageType(MemberInfo viewModelTypeInfo)
        {
            var pageTypeInfo = _appAssembly
                .CreatableTypes()
                .Inherits<IPageBase>()
                .SingleOrDefault(x => x.Name == PageNameFromViewModel(viewModelTypeInfo));

            return pageTypeInfo?.AsType();
        }

        /// <summary>
        /// Gets the name of a view based on the associated view model.
        /// </summary>
        private static string PageNameFromViewModel(MemberInfo viewModelTypeInfo)
        {
            var viewModelName = viewModelTypeInfo.Name;
            if (viewModelName.EndsWith(ViewModelSuffix))
            {
                return viewModelName.Replace(ViewModelSuffix, PageSuffix);
            }

            throw new FormatException($"{viewModelName} is treated as view model but does not end with '{ViewModelSuffix}'. Name should be '{viewModelName}{ViewModelSuffix}', expected corresponding view should be named '{viewModelName}{PageSuffix}'.");
        }

        /// <summary>
        /// Resolve the page associated to the view model.
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>page instance that corresponds to the view model type</returns>
        internal void ReleasePage(BaseViewModel viewModel)
        {
            ReleasePage(viewModel.GetType());
        }

        /// <summary>
        /// Resolve the page associated to the view model.
        /// </summary>
        /// <param name="viewModelType">view model type</param>
        private void ReleasePage(Type viewModelType)
        {
            if (viewModelType == null || ResolvePage(viewModelType)?.GetType().GetCustomAttribute<TransientAttribute>() == null) return;
            var resolver = GetPageResolver(viewModelType);
            resolver?.Release();
        }

        /// <summary>
        /// Resolves page associated with the viewmodel.
        /// </summary>
        /// <param name="viewModelType">view model type</param>
        /// <returns>bounded page</returns>
        private Page ResolvePage(Type viewModelType)
        {
            if (!viewModelType.GetTypeInfo().IsSubclassOf(typeof(BaseViewModel)))
            {
                throw new ArgumentException($"{nameof(viewModelType)} is not a type of BaseViewModel");
            }

            // Check resolver registration.
            CheckResolverRegistration(viewModelType);

            // Obtain page from resolver.
            return GetPageFromResolver(viewModelType);
        }

        /// <summary>
        /// Check resolver registration.
        /// </summary>
        /// <param name="viewModelType"></param>
        private void CheckResolverRegistration(Type viewModelType)
        {
            if (!_pageResolvers.ContainsKey(viewModelType))
            {
                _pageResolvers.Add(viewModelType, new PageResolver(FindPageType(viewModelType)));
            }
        }

        /// <summary>
        /// Get the page from the page resolver.
        /// </summary>
        /// <returns></returns>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        private Page GetPageFromResolver(Type viewModelType)
        {
            var resolver = GetPageResolver(viewModelType);
            return (Page)resolver?.Resolve();
        }

        /// <summary>
        /// Obtain the page resolver associated with the view model type.
        /// </summary>
        /// <param name="viewModelType">view model type</param>
        /// <returns></returns>
        private PageResolver GetPageResolver(Type viewModelType)
        {
            return _pageResolvers.ContainsKey(viewModelType) ? _pageResolvers[viewModelType] : default;
        }
    }
}
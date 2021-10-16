using System.Threading.Tasks;
using CafeLib.Mobile.Services;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Extensions
{
    public static class NavigationExtensions
    {
        /// <summary>
        /// Returns the page at the top of the navigation stack.
        /// </summary>
        /// <param name="service">navigation service</param>
        /// <returns>page at top of navigation service navigation page</returns>
        public static Page Peek(this INavigationService service)
        {
            return service.CurrentPage;
        }

        /// <summary>
        /// Insert viewmodel ahead of another viewmodel
        /// </summary>
        /// <typeparam name="T1">type of view model to insert before</typeparam>
        /// <typeparam name="T2">type of the current view model</typeparam>
        /// <param name="service">navigation service</param>
        /// <returns>awaitable task</returns>
        public static void InsertBefore<T1, T2>(this INavigationService service) where T1 : BaseViewModel where T2 : BaseViewModel
        {
            var vm1 = Application.Current.Resolve<T1>();
            var vm2 = Application.Current.Resolve<T2>();
            service.InsertBefore(vm1, vm2);
        }

        /// <summary>
        /// Navigate to view model.
        /// </summary>
        /// <param name="service">navigation service</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        public static void Navigate<T>(this INavigationService service, bool animate = false) where T : BaseViewModel
        {
            service.Navigate(Application.Current.Resolve<T>(), animate);
        }

        /// <summary>
        /// Navigate to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        public static void Navigate<T>(this INavigationService service, T viewModel, bool animate = false) where T : BaseViewModel
        {
            Application.Current.Resolve<IDeviceService>().RunOnMainThread(async () =>
            {
                await viewModel.Initiate();
                await service.PushAsync(viewModel, animate);
            });
        }

        /// <summary>
        /// Navigate to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        public static void Navigate<T, TP>(this INavigationService service, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            service.Navigate(Application.Current.Resolve<T>(), parameter, animate);
        }

        /// <summary>
        /// Navigate to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        public static void Navigate<T, TP>(this INavigationService service, T viewModel, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            Application.Current.RunOnMainThread(async () =>
            {
                await viewModel.Initiate(parameter);
                await service.PushAsync(viewModel, animate);
            });
        }

        /// <summary>
        /// Navigate asynchronously to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        public static Task NavigateAsync<T>(this INavigationService service, bool animate = false) where T : BaseViewModel
        {
            return service.NavigateAsync(Application.Current.Resolve<T>(), animate);
        }

        /// <summary>
        /// Navigate asynchronously to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        public static Task NavigateAsync<T>(this INavigationService service, T viewModel, bool animate = false) where T : BaseViewModel
        {
            var completionSource = new TaskCompletionSource<bool>();

            Application.Current.RunOnMainThread(async () =>
            {
                await viewModel.Initiate();
                await service.PushAsync(viewModel, animate);
                completionSource.SetResult(true);
            });

            return completionSource.Task;
        }

        /// <summary>
        /// Navigate asynchronously to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        public static Task NavigateAsync<T, TP>(this INavigationService service, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            return service.NavigateAsync(Application.Current.Resolve<T>(), parameter, animate);
        }

        /// <summary>
        /// Navigate asynchronously to view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns>asynchronous task</returns>
        public static Task NavigateAsync<T, TP>(this INavigationService service, T viewModel, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            var completionSource = new TaskCompletionSource<bool>();

            Application.Current.RunOnMainThread(async () =>
            {
                await viewModel.Initiate(parameter);
                await service.PushAsync(viewModel, animate);
                completionSource.SetResult(true);
            });

            return completionSource.Task;
        }

        /// <summary>
        /// Navigate to modal view model.
        /// </summary>
        /// <param name="service">navigation service</param>
        /// <param name="animate">transition animation flag</param>
        public static void NavigateModal<T>(this INavigationService service, bool animate = false) where T : BaseViewModel
        {
            service.NavigateModal(Application.Current.Resolve<T>(), animate);
        }

        /// <summary>
        /// Navigate to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        public static void NavigateModal<T, TP>(this INavigationService service, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            service.NavigateModal(Application.Current.Resolve<T>(), parameter, animate);
        }

        /// <summary>
        /// Navigate to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        public static void NavigateModal<T>(this INavigationService service, T viewModel, bool animate = false) where T : BaseViewModel
        {
            Application.Current.Resolve<IDeviceService>().RunOnMainThread(async () =>
            {
                await viewModel.Initiate();
                await service.PushModalAsync(viewModel, animate);
            });
        }

        /// <summary>
        /// Navigate to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        public static void NavigateModal<T, TP>(this INavigationService service, T viewModel, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            Application.Current.Resolve<IDeviceService>().RunOnMainThread(async () =>
            {
                await viewModel.Initiate(parameter);
                await service.PushModalAsync(viewModel, animate);
            });
        }

        /// <summary>
        /// Navigate asynchronously to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns>asynchronous task</returns>
        public static async Task NavigateModalAsync<T>(this INavigationService service, bool animate = false) where T : BaseViewModel
        {
            await service.NavigateModalAsync(Application.Current.Resolve<T>(), animate);
        }

        /// <summary>
        /// Navigate asynchronously to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns>asynchronous task</returns>
        public static Task NavigateModalAsync<T, TP>(this INavigationService service, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            return service.NavigateModalAsync(Application.Current.Resolve<T>(), parameter, animate);
        }

        /// <summary>
        /// Navigate to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns>asynchronous task</returns>
        public static Task NavigateModalAsync<T>(this INavigationService service, T viewModel, bool animate = false) where T : BaseViewModel
        {
            var completionSource = new TaskCompletionSource<bool>();

            Application.Current.RunOnMainThread(async () =>
            {
                await viewModel.Initiate();
                await service.PushModalAsync(viewModel, animate);
                completionSource.SetResult(true);
            });

            return completionSource.Task;
        }

        /// <summary>
        /// Navigate to modal view model.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <typeparam name="TP">view model parameter type</typeparam>
        /// <param name="service">navigation service</param>
        /// <param name="viewModel">view model</param>
        /// <param name="parameter">view model parameter</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        public static Task NavigateModalAsync<T, TP>(this INavigationService service, T viewModel, TP parameter, bool animate = false) where T : BaseViewModel<TP> where TP : class
        {
            var completionSource = new TaskCompletionSource<bool>();

            Application.Current.Resolve<IDeviceService>().RunOnMainThread(async () =>
            {
                await viewModel.Initiate(parameter);
                await service.PushModalAsync(viewModel, animate);
                completionSource.SetResult(true);
            });

            return completionSource.Task;
        }
    }
}

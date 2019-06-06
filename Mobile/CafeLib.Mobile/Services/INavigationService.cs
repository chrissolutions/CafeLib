using System.Threading.Tasks;
using CafeLib.Mobile.ViewModels;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace CafeLib.Mobile.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigation page.
        /// </summary>
        Page NavigationPage { get; }

        /// <summary>
        /// Insert view model before the current view model.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPage1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TPage2"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="currentViewModel"></param>
        /// <returns></returns>
        [UsedImplicitly]
        Task InsertBeforeAsync<T1, TPage1, T2, TPage2>(T1 viewModel, T2 currentViewModel)
            where T1 : BaseViewModel<TPage1>
            where TPage1 : Page
            where T2 : BaseViewModel<TPage2>
            where TPage2 : Page;

        /// <summary>
        /// Navigate to pushed view model.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task PushAsync(Page page, bool animate = false);

        /// <summary>
        /// Navigate back to popped view model
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>page associated with view model</returns>
        [UsedImplicitly]
        Task<TPage> PopAsync<TPage>(bool animate = false) where TPage : Page;

        /// <summary>
        /// Navigate back to popped view model
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>page associated with view model</returns>
        [UsedImplicitly]
        Task<T> PopAsync<T, TPage>(bool animate = false) where T : BaseViewModel<TPage> where TPage : Page;

        /// <summary>
        /// Set the application navigator.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>previous navigator</returns>
        Page SetNavigationPage(Page page);
    }
}

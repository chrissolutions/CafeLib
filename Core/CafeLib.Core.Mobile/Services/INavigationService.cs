using System;
using System.Threading.Tasks;
using CafeLib.Core.Mobile.ViewModels;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.Services
{
    public interface INavigationService : IServiceProvider
    {
        /// <summary>
        /// Navigation page.
        /// </summary>
        Page NavigationPage { get; }

        /// <summary>
        /// Insert view model before the current view model.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="currentViewModel"></param>
        /// <returns></returns>
        Task InsertBeforeAsync(Page page, Page currentViewModel);

        /// <summary>
        /// Navigate to pushed view model.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        Task PushAsync(Page page, bool animate = false);

        /// <summary>
        /// Navigate back to popped view model
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>page associated with view model</returns>
        Task<TPage> PopAsync<TPage>(bool animate = false) where TPage : Page;

        /// <summary>
        /// Navigate back to popped view model
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>page associated with view model</returns>
        Task<T> PopAsync<T, TPage>(bool animate = false) where T : BaseViewModel<TPage> where TPage : Page;

        /// <summary>
        /// Set the application navigator.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>previous navigator</returns>
        Page SetNavigationPage(Page page);
    }
}

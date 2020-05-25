using System.Threading.Tasks;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Services
{
    public interface INavigationService
    {
        Page CurrentPage { get; }

        NavigationPage CurrentNavigationPage { get; }

        bool IsCurrent<T>(T viewModel) where T : BaseViewModel;

        /// <summary>
        /// Insert viewmodel ahead of another viewmodel
        /// </summary>
        /// <typeparam name="T1">type of view model to insert before</typeparam>
        /// <typeparam name="T2">type of the current view model</typeparam>
        /// <param name="viewModel">view model to insert before</param>
        /// <param name="currentViewModel">current view model</param>
        /// <returns>awaitable task</returns>
        void InsertBefore<T1, T2>(T1 viewModel, T2 currentViewModel) where T1 : BaseViewModel where T2 : BaseViewModel;

        /// <summary>
        /// Navigate to pushed view model.
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        Task PushAsync<T>(T viewModel, bool animate = false) where T : BaseViewModel;

        /// <summary>
        /// Navigate to modal page via view model.
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns>task</returns>
        Task PushModalAsync<T>(T viewModel, bool animate = false) where T : BaseViewModel;

        /// <summary>
        /// Navigate back to popped view model
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>task</returns>
        Task PopAsync(bool animate = false);

        /// <summary>
        /// Navigate back from modal stack
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>task</returns>
        Task PopModalAsync(bool animate = false);

        /// <summary>
        /// Pops all but the root Page off the navigation stack.
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        Task PopToRootAsync(bool animate = false);

        /// <summary>
        /// Remove from navigation stack.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="viewModel">view model</param>
        void Remove<T>(T viewModel) where T : BaseViewModel;
    }
}

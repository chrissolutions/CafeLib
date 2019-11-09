using CafeLib.Mobile.ViewModels;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    public interface IPageBase
    {
        /// <summary>
        /// Get the view model bound to the page.
        /// </summary>
        /// <typeparam name="TViewModel">view model type</typeparam>
        TViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel;

        /// <summary>
        /// Set the binding context to the view model
        /// </summary>
        /// <typeparam name="TViewModel">view model type</typeparam>
        /// <param name="viewModel">viewmodel instance</param>
        void SetViewModel<TViewModel>(TViewModel viewModel) where TViewModel: BaseViewModel;

        /// <summary>
        /// Navigable owner.
        /// </summary>
        INavigableOwner Owner { get; }
    }
}

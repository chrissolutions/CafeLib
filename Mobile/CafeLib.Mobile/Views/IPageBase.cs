using CafeLib.Mobile.ViewModels;

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
        /// Navigable owner.
        /// </summary>
        INavigableOwner Owner { get; }
    }
}

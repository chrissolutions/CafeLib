using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Mobile.Services
{
    public interface IPageService
    {
        /// <summary>
        /// Resolve viewmodel type to is associated view.
        /// </summary>
        /// <typeparam name="TViewModel">view model type</typeparam>
        /// <returns>page instance that corresponds to the view model type</returns>
        Page ResolvePage<TViewModel>() where TViewModel : BaseViewModel;

        /// <summary>
        /// Resolve viewmodel type to is associated view.
        /// </summary>
        /// <returns>page instance that corresponds to the view model</returns>
        Page ResolvePage(BaseViewModel viewModel);
    }
}

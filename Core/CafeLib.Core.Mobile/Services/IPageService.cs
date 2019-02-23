using System;
using CafeLib.Core.Async;
using CafeLib.Core.Mobile.ViewModels;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.Services
{
    public interface IPageService : IServiceProvider, IAsyncInit<IPageService>
    {
        /// <summary>
        /// Binds view model to associated page.
        /// </summary>
        /// <typeparam name="T">BaseViewModel type</typeparam>
        /// <returns>page</returns>
        Page BindViewModel<T>(T viewModel) where T : BaseViewModel;

        /// <summary>
        /// Resolves the page for the view model
        /// </summary>
        /// <typeparam name="T">BaseViewModel</typeparam>
        /// <returns>page</returns>
        Page ResolvePage<T>(T viewModel) where T : BaseViewModel;
    }
}

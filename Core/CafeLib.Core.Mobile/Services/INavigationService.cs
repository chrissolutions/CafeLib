using System;
using System.Threading.Tasks;
using CafeLib.Core.Mobile.ViewModels;

namespace CafeLib.Core.Mobile.Services
{
    public interface INavigationService : IServiceProvider
    {
        /// <summary>
        /// Navigate to pushed view model.
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <param name="animate">transition animation flag</param>
        /// <returns></returns>
        Task<bool> PushAsync(BaseViewModel viewModel, bool animate=false);

        /// <summary>
        /// Navigate back to popped view model
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        /// <returns>page associated with view model</returns>
        Task<T> PopAsync<T>(bool animate = false) where T : BaseViewModel;
    }
}

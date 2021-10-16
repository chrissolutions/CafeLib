using System.Linq;
using CafeLib.Mobile.Services;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Extensions
{
    public static class PageExtensions
    {
        /// <summary>
        /// Releases the page instance via its associated view model.
        /// </summary>
        /// <param name="pageService">page service</param>
        /// <param name="viewModel">view model associated with the page</param>
        internal static void ReleasePage(this IPageService pageService, BaseViewModel viewModel)
        {
            ((MobileService)pageService).ReleasePage(viewModel);
        }

        /// <summary>
        /// Determines whether the page has any toolbar items.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static bool HasToolbarItems(this Page page) => page.ToolbarItems.Any();

        /// <summary>
        /// Determines whether the page is landscape or not.
        /// </summary>
        /// <param name="page">page</param>
        /// <returns>true if landscape; false otherwise</returns>
        public static bool IsLandscape(this Page page) => page.Width > page.Height;

        /// <summary>
        /// Determines whether the page is a navigation page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static bool IsNavigationPage(this Page page) => page is NavigationPage;

        /// <summary>
        /// GetResource from the page.
        /// </summary>
        /// <typeparam name="T">resource type</typeparam>
        /// <param name="page">current page</param>
        /// <param name="name">resource name</param>
        /// <returns></returns>
        public static T GetResource<T>(this Page page, string name)
            => (T)page.Resources[name];

        /// <summary>
        /// Get the view model bound to the page.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="page">current page</param>
        /// <returns></returns>
        public static T GetViewModel<T>(this Page page) where T : BaseViewModel
        {
            return page.BindingContext as T;
        }

        /// <summary>
        /// Set the view model to the binding context of the page.
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <param name="page">current page</param>
        /// <param name="viewModel">view model</param>
        public static void SetViewModel<T>(this Page page, T viewModel) where T : BaseViewModel
        {
            if (page.BindingContext == viewModel) return;
            page.BindingContext = null;
            page.BindingContext = viewModel;
        }
    }
}

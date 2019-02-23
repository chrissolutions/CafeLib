using System.Reflection;
using System.Windows.Input;
using CafeLib.Core.Mobile.ViewModels;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.Extensions
{
    public static class PageExtensions
    {
        /// <summary>
        /// Get the view model bound to the page.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public static T GetViewModel<T>(this Page page) where T : BaseViewModel
        {
            return (T) page.BindingContext;
        }

        /// <summary>
        /// Set the view model to the binding context of the page.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        public static void SetViewModel<T>(this Page page, T viewModel) where T : BaseViewModel
        {
            page.BindingContext = viewModel;
        }

        /// <summary>
        /// Invoke the page viewmodel back command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        public static void InvokeBackCommand<T>(this Page view) where T : BaseViewModel
        {
            var viewModel = view.GetViewModel<T>();
            var viewModelType = viewModel.GetType();
            var propInfo = viewModelType.GetTypeInfo().GetDeclaredProperty("BackCommand");
            var command = (ICommand)propInfo?.GetValue(viewModel);
            command?.Execute(null);
        }
    }
}

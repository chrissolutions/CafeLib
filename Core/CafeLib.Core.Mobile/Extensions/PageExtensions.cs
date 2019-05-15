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
        /// <typeparam name="TPage"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public static T GetViewModel<T, TPage>(this TPage page) where T : BaseViewModel<TPage> where TPage : Page
        {
            return (T) page.BindingContext;
        }

        /// <summary>
        /// Set the view model to the binding context of the page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        public static void SetViewModel(this Page page, AbstractViewModel viewModel)
        {
            if (page.BindingContext != viewModel)
            {
                page.BindingContext = viewModel;
            }
        }

        /// <summary>
        /// Invoke the page viewmodel back command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPage"></typeparam>
        /// <param name="page"></param>
        public static void InvokeBackCommand<T, TPage>(this TPage page) where T : BaseViewModel<TPage> where TPage : Page
        {
            var viewModel = page.GetViewModel<T, TPage>();
            var viewModelType = viewModel.GetType();
            var propInfo = viewModelType.GetTypeInfo().GetDeclaredProperty("BackCommand");
            var command = (ICommand)propInfo?.GetValue(viewModel);
            command?.Execute(null);
        }
    }
}

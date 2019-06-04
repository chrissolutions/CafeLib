using CafeLib.Core.IoC;
using CafeLib.Mobile.Core.Extensions;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.ViewModels
{
    public abstract class BaseViewModel<TPage> : AbstractViewModel where TPage : Page
    {
        /// <summary>
        /// BaseViewModel constructor.
        /// </summary>
        /// <param name="resolver"></param>
        protected BaseViewModel(IServiceResolver resolver)
            : base(resolver)
        {
        }

        /// <summary>
        /// Obtain the page associated with the view model.
        /// </summary>
        /// <returns>page</returns>
        public TPage Page => PageService.ResolvePage<BaseViewModel<TPage>, TPage>();

        /// <summary>
        /// Resolve the page associated with the view model.
        /// </summary>
        /// <returns>page</returns>
        public override Page ResolvePage()
        {
            return Page;
        }

        /// <summary>
        /// Resolve view model
        /// </summary>
        /// <typeparam name="T">view model type</typeparam>
        /// <returns>view model instance</returns>
        [UsedImplicitly]
        protected T ResolveViewModel<T>() where T : AbstractViewModel
        {
            return Resolver.Resolve<T>();
        }

        /// <summary>
        /// Cast view model into its corresponding page.
        /// </summary>
        /// <param name="viewModel"></param>
        public static implicit operator TPage(BaseViewModel<TPage> viewModel)
        {
            viewModel.Page.SetViewModel(viewModel);
            return viewModel.Page;
        }
    }
}

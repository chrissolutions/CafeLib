using CafeLib.Core.Mobile.Extensions;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.ViewModels
{
    public abstract class BaseViewModel<TPage> : AbstractViewModel where TPage : Page
    {
        #region Properties

        /// <summary>
        /// Obtain the page associated with the view model.
        /// </summary>
        /// <returns>page</returns>
        public TPage Page => PageService.ResolvePage<BaseViewModel<TPage>, TPage>();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Obtain the page associated with the view model.
        /// </summary>
        /// <returns>page</returns>
        public override Page ResolvePage()
        {
            return Page;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Cast view model into its corresponding page.
        /// </summary>
        /// <param name="viewModel"></param>
        public static implicit operator TPage(BaseViewModel<TPage> viewModel)
        {
            viewModel.Page.SetViewModel(viewModel);
            return viewModel.Page;
        }

        #endregion 
    }
}

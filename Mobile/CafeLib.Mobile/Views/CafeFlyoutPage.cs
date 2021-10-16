using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    public abstract class CafeFlyoutPage : FlyoutPage, IPageBase, ISoftNavigationPage
    {
        /// <summary>
        /// The viewmodel bound to the page.
        /// </summary>
        protected IServiceResolver Resolver => Application.Current.Resolve<IServiceResolver>();

        /// <summary>
        /// Get the view model bound to the page.
        /// </summary>
        /// <typeparam name="TViewModel">view model type</typeparam>
        public TViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel
        {
            return (TViewModel)(BindingContext as BaseViewModel);
        }

        /// <summary>
        /// Set the binding context to the view model
        /// </summary>
        /// <typeparam name="TViewModel">view model type</typeparam>
        /// <param name="viewModel">viewmodel instance</param>
        public void SetViewModel<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
        {
            BindingContext = viewModel;
        }

        /// <summary>
        /// Process OnAppearing lifecycle event.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (GetMasterDetailViewModel()?.AppearingCommand == null) return;
            await GetMasterDetailViewModel().AppearingCommand.ExecuteAsync();
        }

        /// <summary>
        /// Process OnDisappearing lifecycle event.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (GetMasterDetailViewModel()?.DisappearingCommand == null) return;
            GetMasterDetailViewModel()?.DisappearingCommand.ExecuteAsync();
        }
        
        /// <summary>
        /// Process OnLoad lifecycle event.
        /// </summary>
        protected virtual void OnLoad()
        {
            GetMasterDetailViewModel()?.LoadCommand.Execute(null);
        }

        /// <summary>
        /// Process OnUnload lifecycle event.
        /// </summary>
        protected virtual void OnUnload()
        {
            GetMasterDetailViewModel()?.UnloadCommand.Execute(null);
		}        

        /// <summary>
        /// Process hardware back button press event.
        /// </summary>s
        /// <returns>true: ignore behavior; false: default behavior</returns>
        protected override bool OnBackButtonPressed()
        {
            return GetMasterDetailViewModel()?.BackButtonPressed.Execute(NavigationSource.Hardware) ?? false;
        }

        /// <summary>
        /// Process software back button press event.
        /// </summary>
        /// <returns>true: ignore behavior; false: default behavior</returns>
        public bool OnSoftBackButtonPressed()
        {
            return GetMasterDetailViewModel()?.BackButtonPressed.Execute(NavigationSource.Software) ?? false;
        }

        /// <summary>
        /// Return the proper view model from master-detail context. 
        /// </summary>
        /// <returns></returns>
        private BaseViewModel GetMasterDetailViewModel()
        {
            switch (Detail)
            {
                case NavigationPage navPage:
                    return navPage.CurrentPage.GetViewModel<BaseViewModel>();

                case null:
                    return GetViewModel<BaseViewModel>();

                case Page _:
                    return Detail.GetViewModel<BaseViewModel>();
            }
        }
    }
}
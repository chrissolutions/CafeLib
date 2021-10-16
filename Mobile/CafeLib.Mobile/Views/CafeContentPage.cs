﻿using System;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    public abstract class CafeContentPage : ContentPage, IPageBase
    {
        /// <summary>
        /// The viewmodel bound to the page.
        /// </summary>
        protected IServiceResolver Resolver => Application.Current.Resolve<IServiceResolver>();

        /// <summary>
        /// Page loaded sink.
        /// </summary>
        internal Action Loaded => OnLoad;

        /// <summary>
        /// Page unloaded sink.
        /// </summary>
        internal Action Unloaded => OnUnload;

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
            if (GetViewModel<BaseViewModel>()?.AppearingCommand == null) return;
            await GetViewModel<BaseViewModel>().AppearingCommand.ExecuteAsync();
        }

        /// <summary>
        /// Process OnDisappearing lifecycle event.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (GetViewModel<BaseViewModel>()?.DisappearingCommand == null) return;
            GetViewModel<BaseViewModel>()?.DisappearingCommand.Execute(null);
        }

        /// <summary>
        /// Process OnLoad lifecycle event.
        /// </summary>
        protected virtual void OnLoad()
        {
            GetViewModel<BaseViewModel>()?.LoadCommand.Execute(null);
        }

        /// <summary>
        /// Process OnUnload lifecycle event.
        /// </summary>
        protected virtual void OnUnload()
        {
            GetViewModel<BaseViewModel>()?.UnloadCommand.Execute(null);
        }

        /// <summary>
        /// Process hardware back button press event.
        /// </summary>
        /// <returns>true: ignore behavior; false: default behavior</returns>
        protected override bool OnBackButtonPressed()
        {
            return GetViewModel<BaseViewModel>()?.BackButtonPressed.Execute(NavigationSource.Hardware) ?? false;
        }

        /// <summary>
        /// Process software back button press event.
        /// </summary>
        /// <returns>true: ignore behavior; false: default behavior</returns>
        public virtual bool OnSoftBackButtonPressed()
        {
            return GetViewModel<BaseViewModel>()?.BackButtonPressed.Execute(NavigationSource.Software) ?? false;
        }
    }

    public abstract class CafeContentPage<T> : CafeContentPage, ISoftNavigationPage where T : BaseViewModel
    {
        /// <summary>
        /// The viewmodel bound to the page.
        /// </summary>
        public T ViewModel => GetViewModel<T>();

        /// <summary>
        /// CafeContentPage constructor.
        /// </summary>
        protected CafeContentPage()
        {
            SetViewModel(ResolveViewModel());
        }

        /// <summary>
        /// Resolve view model.
        /// </summary>
        /// <returns></returns>
        protected T ResolveViewModel()
        {
            return Resolver.Resolve<T>();
        }
    }
}

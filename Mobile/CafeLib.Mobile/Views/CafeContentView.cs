using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    [SuppressMessage("ReSharper", "AsyncVoidLambda")]
    public abstract class CafeContentView : ContentView
    {
        private readonly List<Guid> _subscriberHandles;
        private BaseViewModel _viewModel;

        /// <summary>
        /// The viewmodel bound to the page.
        /// </summary>
        protected IServiceResolver Resolver => Application.Current.Resolve<IServiceResolver>();

        /// <summary>
        /// Navigation Service
        /// </summary>
        protected IEventService EventService => Resolver.Resolve<IEventService>();

        /// <summary>
        /// Notify that the content view was loaded.
        /// </summary>
        internal Action Loaded => OnLoad;

        /// <summary>
        /// Notify that the content view was unloaded.
        /// </summary>
        internal Action Unloaded => OnUnload;

        /// <summary>
        /// BaseContextView constructor.
        /// </summary>
        protected CafeContentView()
        {
            _subscriberHandles = new List<Guid>();
        }

        /// <summary>
        /// The viewmodel bound to the view.
        /// </summary>
        public BaseViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel == value) return;
                _viewModel = value;
                BindingContext = _viewModel;
                Application.Current.RunOnMainThread(async () => await _viewModel.Initiate());
            }
        }

        /// <summary>
        /// Process OnLoad lifecycle event.
        /// </summary>
        protected virtual async void OnLoad()
        {
            _subscriberHandles.Clear();
            if (ViewModel == null) return;
            await ViewModel.LoadCommand.ExecuteAsync();
        }

        /// <summary>
        /// Process OnUnload lifecycle event.
        /// </summary>
        protected virtual async void OnUnload()
        {
            try
            {
                if (ViewModel == null) return;
                await ViewModel?.UnloadCommand.ExecuteAsync();
            }
            finally
            {
                _subscriberHandles.ForEach(x => EventService.Unsubscribe(x));
                _subscriberHandles.Clear();
            }
        }

        /// <summary>
        /// Publish an event message.
        /// </summary>
        /// <typeparam name="T">event message type</typeparam>
        /// <param name="message">event message</param>
        protected void PublishEvent<T>(T message) where T : IEventMessage
        {
            EventService.Publish(message);
        }

        /// <summary>
        /// Subscribe an action to an event message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        protected void SubscribeEvent<T>(Action<T> action) where T : IEventMessage
        {
            _subscriberHandles.Add(EventService.SubscribeOnMainThread(action));
        }
    }

    public abstract class CafeContentView<T> : CafeContentView where T : BaseViewModel
    {
        /// <summary>
        /// The view model bound to the view.
        /// </summary>
        public new T ViewModel
        {
            get => base.ViewModel as T;
            set => base.ViewModel = value;
        }
    }
}

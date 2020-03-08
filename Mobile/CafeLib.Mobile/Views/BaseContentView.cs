using System;
using System.Collections.Generic;
using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Effects;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.ViewModels;
using Xamarin.Forms;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    public class BaseContentView : ContentView
    {
        private readonly List<Guid> _subscriberHandles;

        /// <summary>
        /// The viewmodel bound to the page.
        /// </summary>
        protected IServiceResolver Resolver => Application.Current.Resolve<IServiceResolver>();

        /// <summary>
        /// Navigation Service
        /// </summary>
        protected IEventService EventService => Resolver.Resolve<IEventService>();

        /// <summary>
        /// Notify appearance of content view from external source.
        /// </summary>
        public Action Appearing => OnAppearing;

        /// <summary>
        /// Notify disappearance of content view from external source.
        /// </summary>
        public Action Disappearing => OnDisappearing;

        /// <summary>
        /// BaseContextView constructor.
        /// </summary>
        public BaseContentView()
        {
            _subscriberHandles = new List<Guid>();
            var lifecycleEffect = new ViewLifecycleEffect();
            lifecycleEffect.Loaded += (s, e) => OnLoad();
            lifecycleEffect.Unloaded += (s, e) => OnUnload();
            Effects.Add(lifecycleEffect);
        }

        /// <summary>
        /// The viewmodel bound to the view.
        /// </summary>
        public BaseViewModel ViewModel
        {
            get => BindingContext as BaseViewModel;
            set => BindingContext = value;
        }

        /// <summary>
        /// Process OnAppearing lifecycle event.
        /// </summary>
        protected virtual async void OnAppearing()
        {
            if (ViewModel == null) return;
            ViewModel.IsVisible = true;
            await ViewModel.AppearingCommand.ExecuteAsync();
        }

        /// <summary>
        /// Process OnDisappearing lifecycle event.
        /// </summary>
        protected virtual async void OnDisappearing()
        {
            if (ViewModel == null) return;
            await ViewModel.DisappearingCommand.ExecuteAsync();
            ViewModel.IsVisible = false;
        }

        /// <summary>
        /// Process OnLoad lifecycle event.
        /// </summary>
        protected virtual async void OnLoad()
        {
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
        /// Process changes to the binding context.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (ViewModel == null) return;
            Application.Current.RunOnMainThread(async () => await ViewModel.Initialize());
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
}

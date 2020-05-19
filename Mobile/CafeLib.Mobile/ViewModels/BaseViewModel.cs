using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Commands;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.Messages;
using CafeLib.Mobile.Services;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.ViewModels
{
    public abstract class BaseViewModel : ObservableBase
    {
        private readonly List<Guid> _onAppearingSubscribers;
        private readonly List<Guid> _onLoadSubscribers;

        private Func<ICommand, Task> ExecuteCommand { get; }

        protected internal enum LifecycleState
        {
            Initial,
            Appearing,
            Load,
            Close,
            Disappearing,
            Unload
        }

        /// <summary>
        /// Initialize view model.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            Lifecycle = LifecycleState.Initial;
            await InitAsync();
        }

        /// <summary>
        /// BaseViewModel constructor.
        /// </summary>
        protected BaseViewModel()
        {
            _onAppearingSubscribers = new List<Guid>();
            _onLoadSubscribers = new List<Guid>();
            Resolver = Application.Current.Resolve<IServiceResolver>();
            Lifecycle = LifecycleState.Initial;
            AppearingCommand = new XamAsyncCommand(() => { });
            DisappearingCommand = new XamAsyncCommand(() => { });
            CloseCommand = new XamAsyncCommand(() => Close());
            LoadCommand = new XamAsyncCommand(() => { });
            UnloadCommand = new XamAsyncCommand(() => { });
            FocusCommand = new Command(() => { });
            BackButtonPressed = new XamCommand<NavigationSource, bool>(x =>
            {
                Close();
                return true;
            });

            ExecuteCommand = async command =>
            {
                switch (command)
                {
                    case IXamAsyncCommand a:
                        await a.ExecuteAsync();
                        break;

                    default:
                        command?.Execute(null);
                        break;
                }
            };
        }

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        protected virtual async Task InitAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Lifecycle state.
        /// </summary>
        protected internal LifecycleState Lifecycle { get; set; }

        /// <summary>
        /// Service resolver.
        /// </summary>
        protected IServiceResolver Resolver { get; }

        /// <summary>
        /// Page service.
        /// </summary>
        protected IPageService PageService => Resolver.Resolve<IPageService>();

        /// <summary>
        /// Navigation service.
        /// </summary>
        protected INavigationService NavigationService => Resolver.Resolve<INavigationService>();

        /// <summary>
        /// Navigation Service
        /// </summary>
        protected IDeviceService DeviceService => Resolver.Resolve<IDeviceService>();

        /// <summary>
        /// Navigation Service
        /// </summary>
        protected IEventService EventService => Resolver.Resolve<IEventService>();

        /// <summary>
        /// Resolve the associated page.
        /// </summary>
        protected Page Page => PageService.ResolvePage(this);

        /// <summary>
        /// Appearing command.
        /// </summary>
        private IXamAsyncCommand _appearingCommand;
        public IXamAsyncCommand AppearingCommand
        {
            get => _appearingCommand;
            set
            {
                _appearingCommand = new XamAsyncCommand(async () =>
                {
                    if (Lifecycle == LifecycleState.Close) return;

                    Lifecycle = LifecycleState.Appearing;
                    IsVisible = true;
                    ReleaseSubscribers();
                    AddSubscribers();

                    try
                    {
                        IsEnabled = false;
                        await ExecuteCommand(value);
                    }
                    finally
                    {
                        IsEnabled = true;
                    }
                });
            }
        }

        /// <summary>
        /// Disappearing command.
        /// </summary>
        private IXamAsyncCommand _disappearingCommand;
        public IXamAsyncCommand DisappearingCommand
        {
            get => _disappearingCommand;
            set
            {
                _disappearingCommand = new XamAsyncCommand(async () =>
                {
                    try
                    {
                        Lifecycle = LifecycleState.Disappearing;
                        await ExecuteCommand(value);
                    }
                    finally
                    {
                        ReleaseSubscribers();
                        IsVisible = false;
                    }
                });
            }
        }

        /// <summary>
        /// Load command.
        /// </summary>
        private IXamAsyncCommand _loadCommand;
        public IXamAsyncCommand LoadCommand
        {
            get => _loadCommand;
            set
            {
                _loadCommand = new XamAsyncCommand(async () =>
                {
                    try
                    {
                        Lifecycle = LifecycleState.Load;
                        if (!IsLoaded)
                        {
                            await ExecuteCommand(value);
                        }
                    }
                    finally
                    {
                        IsLoaded = true;
                    }
                });
            }
        }

        /// <summary>
        /// Load command.
        /// </summary>
        private IXamAsyncCommand _unloadCommand;
        public IXamAsyncCommand UnloadCommand
        {
            get => _unloadCommand;
            set
            {
                _unloadCommand = new XamAsyncCommand(async () =>
                {
                    try
                    {
                        Lifecycle = LifecycleState.Unload;
                        if (IsLoaded)
                        {
                            await ExecuteCommand(value);
                        }
                    }
                    finally
                    {
                        ReleaseSubscribers();
                        IsLoaded = false;
                    }
                });
            }
        }

        /// <summary>
        /// Back button pressed handler.
        /// </summary>
        public IXamCommand<NavigationSource, bool> BackButtonPressed { get; set; }

        /// <summary>
        /// Close command.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Focus command.
        /// </summary>
        public ICommand FocusCommand { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        private string _title;
        public string Title
        {
            get => _title;
            set => SetValue(ref _title, value);
        }

        /// <summary>
        /// Determines whether input is permitted.
        /// </summary>
        private bool _isEnabled;

        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        /// <summary>
        /// Determines loaded state.
        /// </summary>
        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetValue(ref _isLoaded, value);
        }

        /// <summary>
        /// Determines visibility of the view model
        /// </summary>
        private bool _isVisible;
        public virtual bool IsVisible
        {
            get => _isVisible;
            set => SetValue(ref _isVisible, value);
        }

        /// <summary>
        /// Close the view model.
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        protected virtual void Close(bool animate = false)
        {
            Close(new ViewModelCloseMessage(this), animate);
        }

        /// <summary>
        /// Close view model with message
        /// </summary>
        /// <param name="message">view model close message</param>
        /// <param name="animate">transition animation flag</param>
        protected void Close(ViewModelCloseMessage message, bool animate = false)
        {
            if (Lifecycle == LifecycleState.Close) return;
            Lifecycle = LifecycleState.Close;
            PublishEvent(message);
            Page.Navigation.Close(this, animate);
        }

        /// <summary>
        /// Add event message subscribers.
        /// </summary>
        protected virtual void AddSubscribers()
        {
        }

        /// <summary>
        /// Release event message subscribers.
        /// </summary>
        protected virtual void ReleaseSubscribers()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (Lifecycle)
            {
                case LifecycleState.Appearing:
                case LifecycleState.Disappearing:
                    _onAppearingSubscribers.ForEach(x => EventService.Unsubscribe(x));
                    _onAppearingSubscribers.Clear();
                    break;

                case LifecycleState.Initial:
                case LifecycleState.Load:
                case LifecycleState.Unload:
                    _onLoadSubscribers.ForEach(x => EventService.Unsubscribe(x));
                    _onLoadSubscribers.Clear();
                    break;
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
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (Lifecycle)
            {
                case LifecycleState.Appearing:
                    _onAppearingSubscribers.Add(EventService.SubscribeOnMainThread(action));
                    break;

                case LifecycleState.Initial: 
                case LifecycleState.Load:
                    _onLoadSubscribers.Add(EventService.SubscribeOnMainThread(action));
                    break;
            }
        }

        /// <summary>
        /// Resolves viewmodel to its associated page of the specified type.
        /// </summary>
        /// <typeparam name="TPage">page type</typeparam>
        /// <returns>bounded page</returns>
        protected TPage ResolvePageAs<TPage>() where TPage : Page => PageService.ResolvePage(this) as TPage;

        /// <summary>
        /// Resolves viewmodel to is associated page.
        /// </summary>
        /// <returns>bounded page</returns>
        internal Page ResolvePage() => PageService.ResolvePage(this);

        /// <summary>
        /// Establish view model as the application navigator.
        /// </summary>
        /// <returns></returns>
        public NavigationPage AsNavigator()
        {
            return NavigationService.SetNavigator(this);
        }

        /// <summary>
        /// Displays an alert on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        public Task DisplayAlert(string title, string message, string ok = "OK")
        {
            return Application.Current.AlertDialog(title, message, ok);
        }

        /// <summary>
        /// Displays an alert (simple question) on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        /// <param name="cancel">cancel</param>
        public Task<bool> DisplayConfirm(string title, string message, string ok = "OK", string cancel = "Cancel")
        {
            return Application.Current.ConfirmDialog(title, message, ok, cancel);
        }

        /// <summary>
        /// Displays an action sheet (list of buttons) on the page, asking for user input.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="cancel">cancellation string</param>
        /// <param name="destroy">destroy string</param>
        /// <param name="options">option list</param>
        /// <returns></returns>
        public Task<string> DisplayOptions(string title, string cancel, string destroy, IEnumerable<string> options)
        {
            return Application.Current.OptionsDialog(title, cancel, destroy, options);
        }

        /// <summary>
        /// Releases the page from the associated view model type.
        /// </summary>
        internal void ReleasePage()
        {
            PageService.ReleasePage(this);
        }
    }

    /// <summary>
    /// BaseViewModel with initialization parameter.
    /// </summary>
    /// <typeparam name="TParameter">initialization parameter type</typeparam>
    public abstract class BaseViewModel<TParameter> : BaseViewModel where TParameter : class
    {
        /// <summary>
        /// Initialize view model.
        /// </summary>
        /// <param name="parameter">initialization parameter</param>
        /// <returns></returns>
        public async Task Initialize(TParameter parameter)
        {
            Lifecycle = LifecycleState.Initial;

            if (typeof(TParameter) != typeof(object))
            {
                await InitAsync(parameter);
            }
            else
            {
                await InitAsync();
            }
        }

        /// <summary>
        /// Initialize and pass parameter to the view model.
        /// </summary>
        /// <param name="parameter">parameter passed to view model</param>
        protected virtual async Task InitAsync(TParameter parameter)
        {
            await Task.CompletedTask;
        }
    }
}

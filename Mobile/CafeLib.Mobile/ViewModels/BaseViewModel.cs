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
    public abstract class BaseViewModel : ObservableBase, IDisposable
    {
        private readonly List<Guid> _onInitSubscribers;
        private readonly List<Guid> _onAppearingSubscribers;
        private bool _disposed;

        private Func<ICommand, Task> ExecuteCommand { get; }

        protected internal enum LifecycleState
        {
            Initiate,
            Load,
            Appearing,
            Close,
            Disappearing,
            Unload
        }

        /// <summary>
        /// BaseViewModel constructor.
        /// </summary>
        protected BaseViewModel()
        {
            _onInitSubscribers = new List<Guid>();
            _onAppearingSubscribers = new List<Guid>();
            Resolver = Application.Current.Resolve<IServiceResolver>();
            Lifecycle = LifecycleState.Initiate;
            InitCommand = new XamAsyncCommand(() => { });
            AppearingCommand = new XamAsyncCommand(() => { });
            DisappearingCommand = new XamAsyncCommand(() => { });
            CloseCommand = new XamAsyncCommand(async () => await CloseAsync());
            LoadCommand = new XamAsyncCommand(() => { });
            UnloadCommand = new XamAsyncCommand(() => { });
            FocusCommand = new Command(() => { });
            BackButtonPressed = new BackButtonCommand(x => Close());

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
        /// Initiate the view model.
        /// </summary>
        /// <returns></returns>
        public async Task Initiate()
        {
            await ExecuteCommand(_initCommand);
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
        /// Init command.
        /// </summary>
        private protected ICommand _initCommand;

        public IXamAsyncCommand InitCommand
        {
            protected get => (IXamAsyncCommand)_initCommand;
            set
            {
                _initCommand = new XamAsyncCommand(async () =>
                {
                    Lifecycle = LifecycleState.Initiate;
                    await ExecuteCommand(value);
                });
            }
        }

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
                        if (Lifecycle == LifecycleState.Unload) return;
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
                    Lifecycle = LifecycleState.Load;
                    await ExecuteCommand(value);
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
                        if (Lifecycle != LifecycleState.Unload)
                        {
                            Lifecycle = LifecycleState.Unload;
                            await ExecuteCommand(value);
                        }
                    }
                    finally
                    {
                        ReleaseSubscribers();
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
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="animate"></param>
        protected virtual async void Close(ViewModelCloseMessage message, bool animate = false)
        {
            await CloseAsync(message, animate);
        }

        /// <summary>
        /// Close the view model.
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        protected virtual async Task CloseAsync(bool animate = false)
        {
            await CloseAsync(new ViewModelCloseMessage(this), animate);
        }

        /// <summary>
        /// Close view model with message
        /// </summary>
        /// <param name="message">view model close message</param>
        /// <param name="animate">transition animation flag</param>
        protected async Task CloseAsync(ViewModelCloseMessage message, bool animate = false)
        {
            if (Lifecycle == LifecycleState.Close) return;
            Lifecycle = LifecycleState.Close;
            PublishEvent(message);

            var navigationService = Resolver.Resolve<INavigationService>();
            await navigationService.PopAsync(animate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task CloseToRoot()
        {
            var navigationService = Resolver.Resolve<INavigationService>();
            await navigationService.PopToRootAsync();
        }

        /// <summary>
        /// Close the view model.
        /// </summary>
        /// <param name="animate">transition animation flag</param>
        protected virtual void CloseModal(bool animate = false)
        {
            CloseModal(new ViewModelCloseMessage(this), animate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="animate"></param>
        protected virtual async void CloseModal(ViewModelCloseMessage message, bool animate = false)
        {
            await CloseModalAsync(message, animate);
        }

        protected Task CloseModalAsync(bool animate = false)
        {
            return CloseModalAsync(new ViewModelCloseMessage(this), animate);
        }

        protected async Task CloseModalAsync(ViewModelCloseMessage message, bool animate = false)
        {
            if (Lifecycle == LifecycleState.Close) return;
            Lifecycle = LifecycleState.Close;
            PublishEvent(message);

            var navigationService = Resolver.Resolve<INavigationService>();
            await navigationService.PopModalAsync();
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

                case LifecycleState.Initiate:
                case LifecycleState.Load:
                case LifecycleState.Unload:
                    _onInitSubscribers.ForEach(x => EventService.Unsubscribe(x));
                    _onInitSubscribers.Clear();
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

                case LifecycleState.Initiate: 
                case LifecycleState.Load:
                    _onInitSubscribers.Add(EventService.SubscribeOnMainThread(action));
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

        /// <summary>
        /// Dispose the view model.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual async void Dispose(bool disposing)
        {
            if (!disposing) return;
            await UnloadCommand.ExecuteAsync();
        }
    }

    /// <summary>
    /// BaseViewModel with initialization parameter.
    /// </summary>
    /// <typeparam name="TParameter">initialization parameter type</typeparam>
    public abstract class BaseViewModel<TParameter> : BaseViewModel where TParameter : class
    {
        private Func<ICommand, object, Task> ExecuteCommand { get; }

        /// <summary>
        /// BaseViewModel constructor
        /// </summary>
        /// <typeparam name="TParameter">parameter type</typeparam>
        protected BaseViewModel()
        {
            ExecuteCommand = async (command, parameter) =>
            {
                switch (command)
                {
                    case IXamAsyncCommand<TParameter> a:
                        await a.ExecuteAsync((TParameter)parameter);
                        break;

                    default:
                        command?.Execute(parameter);
                        break;
                }
            };
        }

        /// <summary>
        /// Initialize view model.
        /// </summary>
        /// <param name="parameter">initialization parameter</param>
        /// <returns></returns>
        public async Task Initiate(TParameter parameter)
        {
            var task = typeof(TParameter) != typeof(object) ? InitCommand.ExecuteAsync(parameter) : InitCommand.ExecuteAsync();
            await task;
        }

        /// <summary>
        /// Init command.
        /// </summary>
        public new IXamAsyncCommand<TParameter> InitCommand
        {
            protected get => (IXamAsyncCommand<TParameter>)_initCommand;
            set
            {
                _initCommand = new XamAsyncCommand<TParameter>(async x =>
                {
                    Lifecycle = LifecycleState.Initiate;
                    await ExecuteCommand(value, x);
                });
            }
        }
    }
}

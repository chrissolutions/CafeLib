using System;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Commands
{
    /// <summary>
    /// Xamarin command adapter.
    /// </summary>
    public class XamCommand : XamCommand<object>
    {
        /// <summary>
        /// XamCommand constructor.
        /// </summary>
        /// <param name="action">The action to run when the command executes.</param>
        public XamCommand(Action action)
            : base(p => action())
        {
        }

        /// <summary>
        /// XamCommand constructor.
        /// </summary>
        /// <param name="action">The action to run when the command executes.</param>
        /// <param name="canExecute">The routine determining the execution state of the command.</param>
        public XamCommand(Action action, Func<bool> canExecute)
            : base(p => action(), p => canExecute())
        {
        }
    }

    /// <summary>
    /// Xamarin command generic adapter.
    /// </summary>
    public class XamCommand<T> : IXamCommand<T>
    {
        private readonly Command<T> _command;

        /// <summary>
        /// XamCommand constructor.
        /// </summary>
        /// <param name="action">The action to run when the command executes.</param>
        public XamCommand(Action<T> action)
        {
            _command = new Command<T>(action);
        }

        /// <summary>
        /// XamCommand constructor.
        /// </summary>
        /// <param name="action">The action to run when the command executes.</param>
        /// <param name="canExecute">The routine determining the execution state of the command.</param>
        public XamCommand(Action<T> action, Func<T, bool> canExecute)
        {
            _command = new Command<T>(action, canExecute);
        }

        /// <summary>
        /// Determine ability of the command to be executed.
        /// </summary>
        /// <param name="parameter">command parameter</param>
        /// <returns><c>true</c> command can be executed; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return _command.CanExecute(parameter);
        }

        /// <summary>
        /// Execute command.
        /// </summary>
        /// <param name="parameter">command parameter</param>
        public void Execute(object parameter)
        {
            _command.Execute(parameter);
        }

        /// <summary>
        /// Alter the CanExecute state of the command.
        /// </summary>
        public void ChangeCanExecute()
        {
            _command.ChangeCanExecute();
        }

        /// <summary>
        /// Event handler that sinks to changes of the CanExecute state of the command.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => _command.CanExecuteChanged += value;
            remove => _command.CanExecuteChanged -= value;
        }
    }

    public class XamCommand<TParameter, TResult> : IXamCommand<TParameter, TResult>
    {
        private readonly Func<TParameter, TResult> _command;
        private readonly Func<TParameter, bool> _canExecute;

        /// <summary>
        /// XamCommand constructor.
        /// </summary>
        /// <param name="command">The command function to run when upon execution.</param>
        public XamCommand(Func<TParameter, TResult> command)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _canExecute = x => true;
        }

        /// <summary>
        /// XamCommand constructor.
        /// </summary>
        /// <param name="command">The command function to run when upon execution.</param>
        /// <param name="canExecute">The routine determining the execution state of the command.</param>
        public XamCommand(Func<TParameter, TResult> command, Func<TParameter, bool> canExecute)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="parameter">command parameter</param>
        /// <returns></returns>
        public TResult Execute(TParameter parameter)
        {
            var result = CanExecute(parameter) ? _command.Invoke(parameter) : default;
            ChangeCanExecute();
            return result;
        }

        public bool CanExecute(object parameter) => _canExecute((TParameter)parameter);

        public void Execute(object parameter)
        {
            Execute((TParameter) parameter);            
        }

        public event EventHandler CanExecuteChanged;

        public void ChangeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
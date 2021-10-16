using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace DroneLander.Common
{   
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
 
        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
        
        public void Execute(object parameter)
        {
            _execute();
        }

        [UsedImplicitly]
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
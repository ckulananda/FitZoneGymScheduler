using System;
using System.Windows.Input;

namespace FitZoneGymScheduler.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Action _executeWithoutParameter;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action execute)
        {
            _executeWithoutParameter = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
            else
                _executeWithoutParameter?.Invoke();
        }
    }
}
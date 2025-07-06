using System;
using System.Windows.Input;
using log4net;

namespace UI.Commands
{
    public class RelayCommand : ICommand
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RelayCommand));

        private readonly Action<object> _execute;
        private readonly Predicate<object>? _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;

            _log.Info("RelayCommand initialized.");
        }

        public bool CanExecute(object? parameter)
        {
            bool result = _canExecute == null || _canExecute(parameter);
            _log.Debug($"CanExecute evaluated: {result}");
            return result;
        }

        public void Execute(object? parameter)
        {
            _log.Info("RelayCommand Execute invoked.");
            _execute(parameter);
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            _log.Info("RaiseCanExecuteChanged invoked.");
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
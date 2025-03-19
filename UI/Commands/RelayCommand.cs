using System.Windows.Input;

namespace UI.Commands
{
    public class RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        : ICommand
    {
        public bool CanExecute(object? parameter) => canExecute == null || canExecute(parameter);
        public void Execute(object? parameter) => execute(parameter);
        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
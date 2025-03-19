using System.Windows.Input;
using UI.Commands;

namespace UI.ViewModels;

public class RemoveCommandsViewModel
{
    private readonly AddTourViewModel _viewModel;

    public ICommand RemoveTourCommand { get; }
    public ICommand RemoveTourLogCommand { get; }

    public RemoveCommandsViewModel(AddTourViewModel viewModel)
    {
        _viewModel = viewModel;
        RemoveTourCommand = new RelayCommand(RemoveTour, CanRemoveTour);

        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(AddTourViewModel.SelectedTour))
            {
                (RemoveTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        };
    }

    private bool CanRemoveTour(object parameter) => _viewModel.SelectedTour != null;

    private void RemoveTour(object parameter)
    {
        if (_viewModel.SelectedTour != null)
        {
            _viewModel.Tours.Remove(_viewModel.SelectedTour);
            _viewModel.SelectedTour = null;
        }
    }
}
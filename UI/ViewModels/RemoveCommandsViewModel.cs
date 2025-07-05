using System.Windows.Input;
using UI.Commands;
using BL.Services;

namespace UI.ViewModels;

public class RemoveCommandsViewModel
{
    private readonly AddTourViewModel _viewModel;
    private readonly ITourService _tourService;

    public ICommand RemoveTourCommand { get; }
    public ICommand RemoveTourLogCommand { get; }

    public RemoveCommandsViewModel(AddTourViewModel viewModel, ITourService tourService)
    {
        _viewModel = viewModel;
        _tourService = tourService;

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

    private async void RemoveTour(object parameter)
    {
        if (_viewModel.SelectedTour != null)
        {
            var tourToRemove = _viewModel.SelectedTour;

            await _tourService.DeleteTourAsync(tourToRemove.Id);

            _viewModel.Tours.Remove(tourToRemove);
            _viewModel.SelectedTour = null;
        }
    }
}
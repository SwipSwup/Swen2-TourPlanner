using System;
using System.Windows.Input;
using UI.Commands;
using BL.Services;
using log4net;

namespace UI.ViewModels;

public class RemoveCommandsViewModel
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(RemoveCommandsViewModel));

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
            Log.Info($"Attempting to remove tour with ID: {tourToRemove.Id}, Name: {tourToRemove.Name}");

            try
            {
                await _tourService.DeleteTourAsync(tourToRemove.Id);

                _viewModel.Tours.Remove(tourToRemove);
                _viewModel.SelectedTour = null;

                Log.Info($"Successfully removed tour with ID: {tourToRemove.Id}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error while removing tour with ID: {tourToRemove.Id}", ex);
            }
        }
        else
        {
            Log.Warn("RemoveTour was called but SelectedTour was null.");
        }
    }
}
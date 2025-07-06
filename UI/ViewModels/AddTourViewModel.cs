using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BL.DTOs;
using log4net;

namespace UI.ViewModels;

public abstract class AddTourViewModel : INotifyPropertyChanged
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(AddTourViewModel));

    private TourDto _selectedTour;
    private TourLogDto _selectedTourLog;
    private ObservableCollection<TourDto> _tours;

    public ObservableCollection<TourDto> Tours
    {
        get => _tours;
        set
        {
            if (_tours != value)
            {
                _log.Info($"Tours collection changed. New count = {value?.Count ?? 0}");
                _tours = value;
                OnPropertyChanged();
            }
        }
    }

    public TourDto SelectedTour
    {
        get => _selectedTour;
        set
        {
            if (_selectedTour != value)
            {
                _log.Info($"SelectedTour changed from '{_selectedTour?.Name}' to '{value?.Name}'");
                _selectedTour = value;
                OnPropertyChanged();
            }
        }
    }

    public TourLogDto SelectedTourLog
    {
        get => _selectedTourLog;
        set
        {
            if (_selectedTourLog != value)
            {
                _log.Info($"SelectedTourLog changed.");
                _selectedTourLog = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        _log.Debug($"PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public AddTourViewModel(TourDto selectedTour, TourLogDto selectedTourLog, ObservableCollection<TourDto> tours)
    {
        _log.Info("Initializing AddTourViewModel");
        _selectedTour = selectedTour;
        _selectedTourLog = selectedTourLog;
        _tours = tours;

        Tours = tours ?? new ObservableCollection<TourDto>();
        _log.Debug($"Constructor completed. Tours count: {Tours.Count}");
    }
}

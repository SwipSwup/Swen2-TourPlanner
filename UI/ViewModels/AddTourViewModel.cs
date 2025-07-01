using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BL.Models;

namespace UI.ViewModels;

public abstract class AddTourViewModel : INotifyPropertyChanged
{
    private TourDto _selectedTour;
    private TourLogDto _selectedTourLog;
    private ObservableCollection<TourDto> _tours;

    public ObservableCollection<TourDto> Tours
    {
        get => _tours;
        init
        {
            if (_tours != value)
            {
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
                _selectedTourLog = value;
                OnPropertyChanged();
            }
        }
    }

    // Implement INotifyPropertyChanged here
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public AddTourViewModel(TourDto selectedTour, TourLogDto selectedTourLog, ObservableCollection<TourDto> tours)
    {
        _selectedTour = selectedTour;
        _selectedTourLog = selectedTourLog;
        _tours = tours;
        // Initialize the Tours collection
        Tours = new ObservableCollection<TourDto>();
    }
}
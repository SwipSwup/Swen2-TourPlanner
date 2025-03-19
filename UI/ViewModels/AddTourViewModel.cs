using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BL.Models;

public class AddTourViewModel : INotifyPropertyChanged
{
    private Tour _selectedTour;
    private TourLog _selectedTourLog;
    private ObservableCollection<Tour> _tours;

    public ObservableCollection<Tour> Tours
    {
        get => _tours;
        set
        {
            if (_tours != value)
            {
                _tours = value;
                OnPropertyChanged();
            }
        }
    }

    public Tour SelectedTour
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

    public TourLog SelectedTourLog
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
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public AddTourViewModel()
    {
        // Initialize the Tours collection
        Tours = new ObservableCollection<Tour>();
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TourPlanner;
using TourPlanner.UI.Commands;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Tour> Tours { get; set; }
    public ICommand AddTourCommand { get; }
    public ICommand AddTourLogCommand { get; }
    public ICommand RemoveTourLogCommand { get; }
    public ICommand EditTourLogCommand { get; }

    private Tour _selectedTour;
    public Tour SelectedTour
    {
        get => _selectedTour;
        set
        {
            _selectedTour = value;
            NotifyPropertyChanged(nameof(SelectedTour));
            NotifyPropertyChanged(nameof(TourLogs)); // Update TourLogs when the selected tour changes
        }
    }

    public ObservableCollection<TourLog> TourLogs => SelectedTour?.TourLogs;

    private TourLog _selectedTourLog;  // Add this property to track the selected TourLog
    public TourLog SelectedTourLog
    {
        get => _selectedTourLog;
        set
        {
            _selectedTourLog = value;
            NotifyPropertyChanged(nameof(SelectedTourLog)); // Notify when SelectedTourLog changes
        }
    }

    public MainViewModel()
    {
        Tours = new ObservableCollection<Tour>();
        AddTourCommand = new RelayCommand(AddTour);
        AddTourLogCommand = new RelayCommand(AddTourLog);
        RemoveTourLogCommand = new RelayCommand(RemoveTourLog);
    }

    private void AddTour(object parameter)
    {
        var addTourWindow = new AddTourWindow();
        if (addTourWindow.ShowDialog() == true)
        {
            var newTour = new Tour
            {
                Name = addTourWindow.TourName,
                Description = addTourWindow.TourDescription,
                From = addTourWindow.From,
                To = addTourWindow.To,
                TransportType = addTourWindow.TransportType,
                Distance = addTourWindow.Distance,
                EstimatedTime = addTourWindow.EstimatedTime,
                ImagePath = addTourWindow.ImagePath,
            };

            Tours.Add(newTour);
        }
    }

    private void AddTourLog(object parameter)
    {
        if (SelectedTour != null)
        {
            var addTourLogWindow = new AddTourLogWindow();
            if (addTourLogWindow.ShowDialog() == true)
            {
                var newLog = new TourLog
                {
                    Date = addTourLogWindow.Date,
                    Duration = addTourLogWindow.Duration,
                    Distance = addTourLogWindow.Distance
                };

                SelectedTour.TourLogs.Add(newLog);
            }
        }
    }

    private void RemoveTourLog(object parameter)
    {
        if (SelectedTour != null && SelectedTourLog != null)
        {
            SelectedTour.TourLogs.Remove(SelectedTourLog); // Remove the selected TourLog
            SelectedTourLog = null; // Deselect the TourLog
        }
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

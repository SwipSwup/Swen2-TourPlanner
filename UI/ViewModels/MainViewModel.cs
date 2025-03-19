using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BL.Models;
using TourPlanner;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Views;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Tour> Tours { get; set; }
    public ICommand AddTourCommand { get; }
    public ICommand AddTourLogCommand { get; }
    public ICommand RemoveTourLogCommand { get; }
    public ICommand EditTourLogCommand { get; }
    public ICommand OpenEditTourWindowCommand { get; }
    public ICommand OpenRemoveTourWindowCommand { get; }

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
        OpenEditTourWindowCommand = new RelayCommand(OpenEditTourWindow);
        OpenRemoveTourWindowCommand = new RelayCommand(OpenRemoveTourWindow);
    }

    private void OpenEditTourWindow(object parameter)
    {
        if (SelectedTour != null)
        {
            // Open the EditTourWindow and pass the selected tour
            var editTourWindow = new EditTourWindow(SelectedTour);
            if (editTourWindow.ShowDialog() == true)
            {
                // After editing, update the list (if necessary)
                NotifyPropertyChanged(nameof(Tours));
                NotifyPropertyChanged(nameof(SelectedTour));
            }
        }
    }


    private void OpenRemoveTourWindow(object parameter)
    {
        var removeToursWindow = new RemoveToursWindow(Tours);
        removeToursWindow.ShowDialog();
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

            // Validierung aufrufen
            var errors = newTour.Validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Tour wird nicht hinzugefügt, wenn Fehler vorhanden sind
            }

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

                // Validate the new TourLog before adding
                var errors = newLog.Validate();
                if (errors.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Don't add the log if there are errors
                }

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

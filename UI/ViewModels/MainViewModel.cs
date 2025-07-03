using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BL.DTOs;
using BL.Services;
using UI.Commands;

namespace UI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ITourService _tourService;

    public ObservableCollection<TourDto> Tours { get; set; }

    public ICommand AddTourCommand { get; }
    public ICommand AddTourLogCommand { get; }
    public ICommand RemoveTourLogCommand { get; }
    public ICommand EditTourLogCommand { get; }
    public ICommand OpenEditTourWindowCommand { get; }
    public ICommand OpenRemoveTourWindowCommand { get; }

    private TourDto? _selectedTour;
    public TourDto? SelectedTour
    {
        get => _selectedTour;
        set
        {
            if (_selectedTour != value)
            {
                _selectedTour = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TourLogs));
            }
        }
    }

    public ObservableCollection<TourLogDto>? TourLogs => SelectedTour != null
        ? new ObservableCollection<TourLogDto>(SelectedTour.TourLogs ?? new List<TourLogDto>())
        : null;

    private TourLogDto? _selectedTourLog;
    public TourLogDto? SelectedTourLog
    {
        get => _selectedTourLog;
        set
        {
            if (_selectedTourLog != value)
            {
                _selectedTourLog = value;
                NotifyPropertyChanged();
            }
        }
    }

    public MainViewModel(ITourService tourService)
    {
        _tourService = tourService;

        Tours = new ObservableCollection<TourDto>();

        AddTourCommand = new RelayCommand(async _ => await AddTour());
        AddTourLogCommand = new RelayCommand(_ => AddTourLog());
        RemoveTourLogCommand = new RelayCommand(_ => RemoveTourLog());
        OpenEditTourWindowCommand = new RelayCommand(_ => OpenEditTourWindow());
        OpenRemoveTourWindowCommand = new RelayCommand(_ => OpenRemoveTourWindow());
    }

    private async System.Threading.Tasks.Task AddTour()
    {
        var addTourWindow = new AddTourWindow();
        if (addTourWindow.ShowDialog() == true)
        {
            var newTour = new TourDto
            {
                Name = addTourWindow.TourName,
                Description = addTourWindow.TourDescription,
                From = addTourWindow.From,
                To = addTourWindow.To,
                TransportType = addTourWindow.TransportType,
                Distance = addTourWindow.Distance,
                EstimatedTime = addTourWindow.EstimatedTime
            };

            var errors = newTour.Validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _tourService.CreateTourAsync(newTour);

                // Reload tours from DB to get updated list
                var tours = await _tourService.GetAllToursAsync();
                Tours.Clear();
                foreach (var t in tours)
                    Tours.Add(t);

                NotifyPropertyChanged(nameof(Tours));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating tour: {ex.Message}");
            }
        }
    }

    private void AddTourLog()
    {
        if (SelectedTour != null)
        {
            var addTourLogWindow = new AddTourLogWindow();
            if (addTourLogWindow.ShowDialog() == true)
            {
                // TODO: Implement adding tour log logic, e.g.:
                /*
                var newLog = new TourLogDto
                {
                    DateTime = addTourLogWindow.DateTime,
                    Duration = addTourLogWindow.Duration,
                    TotalDistance = addTourLogWindow.TotalDistance,
                    Comment = addTourLogWindow.Comment,
                    Difficulty = addTourLogWindow.Difficulty,
                    TotalTime = addTourLogWindow.TotalTime,
                    Rating = addTourLogWindow.Rating
                };

                var errors = newLog.Validate();
                if (errors.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedTour.TourLogs.Add(newLog);
                NotifyPropertyChanged(nameof(TourLogs));
                */
            }
        }
    }

    private void RemoveTourLog()
    {
        if (SelectedTour != null && SelectedTourLog != null)
        {
            SelectedTour.TourLogs.Remove(SelectedTourLog);
            SelectedTourLog = null;
            NotifyPropertyChanged(nameof(TourLogs));
        }
    }

    private void OpenEditTourWindow()
    {
        if (SelectedTour != null)
        {
            var editTourWindow = new EditTourWindow(SelectedTour);
            if (editTourWindow.ShowDialog() == true)
            {
                NotifyPropertyChanged(nameof(Tours));
                NotifyPropertyChanged(nameof(SelectedTour));
                NotifyPropertyChanged(nameof(TourLogs));
            }
        }
    }

    private void OpenRemoveTourWindow()
    {
        var removeToursWindow = new RemoveToursWindow(Tours);
        removeToursWindow.ShowDialog();
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

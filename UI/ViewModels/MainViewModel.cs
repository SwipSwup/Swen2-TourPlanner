using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BL.DTOs;
using BL.Services;
using Microsoft.Win32;
using UI.Commands;
using UI.Views;
using log4net;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));
        private readonly ITourService _tourService;
        private readonly IReportService _reportService;

        public ObservableCollection<TourDto> Tours { get; set; }

        public ICommand AddTourCommand { get; }
        public ICommand AddTourLogCommand { get; }
        public ICommand EditTourLogCommand { get; }
        public ICommand RemoveTourLogCommand { get; }
        public ICommand OpenEditTourWindowCommand { get; }
        public ICommand OpenRemoveTourWindowCommand { get; }
        public ICommand ImportToursCommand { get; }
        public ICommand ExportToursCommand { get; }
        public RelayCommand GenerateSummaryReportCommand { get; }
        public RelayCommand GenerateTourReportCommand { get; }

        public ICommand AddRandomTourCommand { get; }

        private static readonly List<string> EuropeanCities = new()
        {
            "Vienna", "Paris", "Berlin", "Rome", "Madrid", "Lisbon", "Amsterdam",
            "Brussels", "Copenhagen", "Dublin", "Oslo", "Stockholm", "Helsinki",
            "Warsaw", "Budapest", "Prague", "Zurich", "Munich", "Hamburg", "Barcelona",
            "Milan", "Venice", "Athens", "Edinburgh", "London", "Manchester", "Birmingham",
            "Glasgow", "Reykjavik", "Tallinn", "Riga", "Vilnius", "Ljubljana", "Bratislava",
            "Luxembourg", "Valletta", "San Marino", "Monaco", "Andorra la Vella", "Belgrade",
            "Zagreb", "Sarajevo", "Skopje", "Podgorica", "Pristina", "Tirana", "Sofia",
            "Bucharest", "Chisinau", "Kiev", "Minsk", "Moscow", "St. Petersburg", "Istanbul",
            "Ankara", "Nice", "Lyon", "Marseille", "Bordeaux", "Seville", "Porto", "Genoa",
            "Naples", "Florence", "Turin", "Palermo", "Valencia", "Bilbao", "Malaga", "Granada",
            "Split", "Dubrovnik", "Krakow", "Gdansk", "Lodz", "Katowice", "Wroclaw", "Poznan",
            "Vilnius", "Kaunas", "Tallinn", "Riga", "Paphos", "Limassol", "Funchal", "Madeira",
            "Canary Islands", "Corsica", "Sardinia", "Sicily", "Crete", "Rhodes", "Santorini"
        };

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
                    GenerateTourReportCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<TourLogDto>? TourLogs =>
            SelectedTour != null
                ? new ObservableCollection<TourLogDto>(SelectedTour.TourLogs ?? new System.Collections.Generic.List<TourLogDto>())
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
                    ((RelayCommand)EditTourLogCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public MainViewModel(ITourService tourService, IReportService reportService)
        {
            _tourService = tourService;
            _reportService = reportService;

            Tours = new ObservableCollection<TourDto>();

            AddTourCommand = new RelayCommand(async _ => await AddTour());
            AddTourLogCommand = new RelayCommand(async _ => await AddTourLogAsync());
            RemoveTourLogCommand = new RelayCommand(async _ => await RemoveTourLogAsync());
            OpenEditTourWindowCommand = new RelayCommand(_ => OpenEditTourWindow());
            OpenRemoveTourWindowCommand = new RelayCommand(_ => OpenRemoveTourWindow());
            EditTourLogCommand = new RelayCommand(async _ => await UpdateTourLogAsync(), _ => SelectedTourLog != null);

            ImportToursCommand = new RelayCommand(async _ => await ImportToursAsync());
            ExportToursCommand = new RelayCommand(async _ => await ExportToursAsync());
            GenerateSummaryReportCommand = new RelayCommand(async _ => await GenerateSummaryReportAsync());
            GenerateTourReportCommand = new RelayCommand(async _ => await GenerateTourReportAsync(), _ => SelectedTour != null);

            AddRandomTourCommand = new RelayCommand(async _ => await AddRandomTour());

            _ = LoadToursAsync();
        }

        private async Task LoadToursAsync()
        {
            log.Info("Loading tours...");
            var tours = await _tourService.GetAllToursAsync();
            Tours.Clear();
            foreach (var t in tours)
                Tours.Add(t);
            NotifyPropertyChanged(nameof(Tours));
            log.Info("Tours loaded successfully.");
        }

        private async Task AddTour()
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
                    log.Info($"Created tour '{newTour.Name}'");
                    await LoadToursAsync();
                }
                catch (Exception ex)
                {
                    log.Error("Error creating tour", ex);
                    MessageBox.Show($"Error creating tour: {ex.Message}");
                }
            }
        }

        internal async Task AddRandomTour()
        {
            var window = new AddRandomTourWindow();
            if (window.ShowDialog() == true)
            {
                var random = new Random();
                string randomDestination = EuropeanCities[random.Next(EuropeanCities.Count)];
                string name = $"{window.From} to {randomDestination}";
                string finalDescription = $"{window.UserDescription?.Trim()}\nThis is a trip from {window.From} to {randomDestination}.";

                var newTour = new TourDto
                {
                    Name = name,
                    Description = finalDescription,
                    From = window.From,
                    To = randomDestination,
                    TransportType = window.TransportType
                };

                try
                {
                    await _tourService.CreateTourAsync(newTour);
                    log.Info($"Random tour added: {name}");
                    await LoadToursAsync();
                }
                catch (Exception ex)
                {
                    log.Error("Error adding random tour", ex);
                    MessageBox.Show($"Error adding random tour: {ex.Message}");
                }
            }
        }

        private async Task AddTourLogAsync()
        {
            if (SelectedTour == null)
            {
                MessageBox.Show("Please select a tour first.", "No Tour Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addLogWindow = new AddTourLogWindow();
            if (addLogWindow.ShowDialog() == true)
            {
                var newLog = new TourLogDto
                {
                    Comment = addLogWindow.Comment,
                    DateTime = addLogWindow.Date,
                    Difficulty = addLogWindow.Difficulty,
                    TotalDistance = addLogWindow.TotalDistance,
                    TotalTime = addLogWindow.Duration,
                    Rating = addLogWindow.Rating
                };

                try
                {
                    await _tourService.AddTourLogAsync(SelectedTour.Id, newLog);
                    log.Info($"Added log to tour '{SelectedTour.Name}'");

                    var updatedTour = (await _tourService.GetAllToursAsync())
                        .FirstOrDefault(t => t.Id == SelectedTour.Id);
                    if (updatedTour != null)
                    {
                        SelectedTour.TourLogs = updatedTour.TourLogs;
                        NotifyPropertyChanged(nameof(TourLogs));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error adding tour log", ex);
                    MessageBox.Show($"Error adding tour log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task UpdateTourLogAsync()
        {
            if (SelectedTourLog == null)
                return;

            var editLogWindow = new EditTourLogWindow(SelectedTourLog);
            if (editLogWindow.ShowDialog() == true)
            {
                SelectedTourLog.DateTime = editLogWindow.Date;
                SelectedTourLog.TotalTime = editLogWindow.Duration;
                SelectedTourLog.TotalDistance = editLogWindow.TotalDistance;
                SelectedTourLog.Comment = editLogWindow.Comment;
                SelectedTourLog.Difficulty = editLogWindow.Difficulty;
                SelectedTourLog.Rating = editLogWindow.Rating;

                try
                {
                    await _tourService.UpdateTourLogAsync(SelectedTourLog);
                    log.Info($"Updated log for tour '{SelectedTour?.Name}'");

                    var updatedTour = (await _tourService.GetAllToursAsync())
                        .FirstOrDefault(t => t.Id == SelectedTour.Id);
                    if (updatedTour != null)
                    {
                        SelectedTour.TourLogs = updatedTour.TourLogs;
                        NotifyPropertyChanged(nameof(TourLogs));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error updating tour log", ex);
                    MessageBox.Show($"Error updating tour log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task RemoveTourLogAsync()
        {
            if (SelectedTour == null || SelectedTourLog == null)
            {
                MessageBox.Show("Please select a tour and a tour log to delete.", "Delete Tour Log", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete the selected tour log?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                await _tourService.DeleteTourLogAsync(SelectedTourLog.Id);
                SelectedTour.TourLogs.Remove(SelectedTourLog);
                log.Info($"Deleted tour log from tour '{SelectedTour.Name}'");
                SelectedTourLog = null;
                NotifyPropertyChanged(nameof(TourLogs));
            }
            catch (Exception ex)
            {
                log.Error("Error deleting tour log", ex);
                MessageBox.Show($"Error deleting tour log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OpenEditTourWindow()
        {
            if (SelectedTour == null) return;

            var editTourWindow = new EditTourWindow(SelectedTour);
            if (editTourWindow.ShowDialog() == true)
            {
                try
                {
                    await _tourService.UpdateTourAsync(editTourWindow.EditedTour);
                    log.Info($"Updated tour: {editTourWindow.EditedTour.Name}");
                    await LoadToursAsync();
                    SelectedTour = Tours.FirstOrDefault(t => t.Id == editTourWindow.EditedTour.Id);
                }
                catch (Exception ex)
                {
                    log.Error("Error updating tour", ex);
                    MessageBox.Show($"Error updating tour: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenRemoveTourWindow()
        {
            var removeWindow = new RemoveToursWindow(Tours, _tourService);
            if (removeWindow.ShowDialog() == true)
            {
                log.Info("Removed one or more tours.");
                _ = LoadToursAsync();
            }
        }

        private async Task ImportToursAsync()
        {
            var dlg = new OpenFileDialog { Filter = "JSON files (*.json)|*.json", Title = "Import Tours from JSON" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    await _tourService.ImportToursFromJsonAsync(dlg.FileName);
                    log.Info($"Imported tours from file: {dlg.FileName}");
                    await LoadToursAsync();
                    MessageBox.Show("Tours imported successfully.");
                }
                catch (Exception ex)
                {
                    log.Error("Error importing tours", ex);
                    MessageBox.Show($"Error importing tours: {ex.Message}");
                }
            }
        }

        private async Task ExportToursAsync()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Export All Tours as JSON",
                FileName = "AllTours.json"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var allTours = await _tourService.GetAllToursAsync();
                    var json = System.Text.Json.JsonSerializer.Serialize(allTours, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    await System.IO.File.WriteAllTextAsync(dlg.FileName, json);
                    log.Info($"Exported tours to file: {dlg.FileName}");
                    MessageBox.Show("Tours exported successfully.");
                }
                catch (Exception ex)
                {
                    log.Error("Error exporting tours", ex);
                    MessageBox.Show($"Error exporting tours: {ex.Message}");
                }
            }
        }

        private async Task GenerateSummaryReportAsync()
        {
            try
            {
                var allTours = await _tourService.GetAllToursAsync();
                await _reportService.GenerateSummaryReportAsync(allTours);
                log.Info("Generated summary report.");
                MessageBox.Show("Summary report generated.");
            }
            catch (Exception ex)
            {
                log.Error("Error generating summary report", ex);
                MessageBox.Show($"Error generating summary report: {ex.Message}");
            }
        }

        private async Task GenerateTourReportAsync()
        {
            if (SelectedTour == null) return;

            try
            {
                await _reportService.GenerateTourReportAsync(SelectedTour);
                log.Info($"Generated tour report for {SelectedTour.Name}");
                MessageBox.Show($"Tour report generated for {SelectedTour.Name}.");
            }
            catch (Exception ex)
            {
                log.Error("Error generating tour report", ex);
                MessageBox.Show($"Error generating tour report: {ex.Message}");
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

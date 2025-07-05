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

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITourService _tourService;
        private readonly IReportService _reportService;

        public ObservableCollection<TourDto> Tours { get; set; }

        public ICommand AddTourCommand { get; }
        public ICommand AddTourLogCommand { get; }
        public ICommand RemoveTourLogCommand { get; }
        public ICommand OpenEditTourWindowCommand { get; }
        public ICommand OpenRemoveTourWindowCommand { get; }

        public ICommand ImportToursCommand { get; }
        public ICommand ExportToursCommand { get; }
        public RelayCommand GenerateSummaryReportCommand { get; }
        public RelayCommand GenerateTourReportCommand { get; }

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
                }
            }
        }

        public MainViewModel(ITourService tourService, IReportService reportService)
        {
            _tourService = tourService;
            _reportService = reportService;

            Tours = new ObservableCollection<TourDto>();

            AddTourCommand = new RelayCommand(async _ => await AddTour());
            AddTourLogCommand = new RelayCommand(_ => AddTourLog());
            RemoveTourLogCommand = new RelayCommand(_ => RemoveTourLog());
            OpenEditTourWindowCommand = new RelayCommand(_ => OpenEditTourWindow());
            OpenRemoveTourWindowCommand = new RelayCommand(_ => OpenRemoveTourWindow());

            ImportToursCommand = new RelayCommand(async _ => await ImportToursAsync());
            ExportToursCommand = new RelayCommand(async _ => await ExportToursAsync());
            GenerateSummaryReportCommand = new RelayCommand(async _ => await GenerateSummaryReportAsync());
            GenerateTourReportCommand = new RelayCommand(
                async _ => await GenerateTourReportAsync(),
                _ => SelectedTour != null
            );

            _ = LoadToursAsync();
        }

        private async Task LoadToursAsync()
        {
            var tours = await _tourService.GetAllToursAsync();
            Tours.Clear();
            foreach (var t in tours)
                Tours.Add(t);

            NotifyPropertyChanged(nameof(Tours));
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
                    await LoadToursAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating tour: {ex.Message}");
                }
            }
        }

        private async void AddTourLog()
        {
            if (SelectedTour == null)
            {
                MessageBox.Show("Please select a tour first.", "No Tour Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addLogWindow = new AddTourLogWindow();
            bool? result = addLogWindow.ShowDialog();
            if (result == true)
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

                    // Reload tours or just refresh logs for the selected tour
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
                    MessageBox.Show($"Error adding tour log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void OpenEditTourWindow()
        {
            if (SelectedTour != null)
            {
                var editTourWindow = new EditTourWindow(SelectedTour);
                if (editTourWindow.ShowDialog() == true)
                {
                    try
                    {
                        await _tourService.UpdateTourAsync(editTourWindow.EditedTour);
                        await LoadToursAsync();

                        SelectedTour = Tours.FirstOrDefault(t => t.Id == editTourWindow.EditedTour.Id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating tour: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void OpenRemoveTourWindow()
        {
            var removeWindow = new RemoveToursWindow(Tours, _tourService);
            if (removeWindow.ShowDialog() == true)
            {
                _ = LoadToursAsync();
            }
        }

        private async Task ImportToursAsync()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Import Tours from JSON"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    await _tourService.ImportToursFromJsonAsync(dlg.FileName);
                    await LoadToursAsync();
                    MessageBox.Show("Tours imported successfully.");
                }
                catch (Exception ex)
                {
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
                    MessageBox.Show("Tours exported successfully.");
                }
                catch (Exception ex)
                {
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
                MessageBox.Show("Summary report generated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating summary report: {ex.Message}");
            }
        }

        private async Task GenerateTourReportAsync()
        {
            if (SelectedTour == null) return;

            try
            {
                await _reportService.GenerateTourReportAsync(SelectedTour);
                MessageBox.Show($"Tour report generated for {SelectedTour.Name}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating tour report: {ex.Message}");
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

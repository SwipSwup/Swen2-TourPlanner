using System;
using System.Windows;
using BL.Services;
using UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using log4net;
using System.Reflection;

namespace UI
{
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITourService _tourService;
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            log.Info("Initializing MainWindow.");

            _tourService = App.AppHost.Services.GetRequiredService<ITourService>();
            _viewModel = App.AppHost.Services.GetRequiredService<MainViewModel>();

            DataContext = _viewModel;

            LoadTours();
        }

        private async void LoadTours()
        {
            try
            {
                log.Info("Loading tours from service.");
                var tours = await _tourService.GetAllToursAsync();

                _viewModel.Tours.Clear();
                foreach (var tour in tours)
                {
                    _viewModel.Tours.Add(tour);
                }
                log.Info($"Loaded {tours.Count} tours successfully.");
            }
            catch (Exception e)
            {
                log.Error("Failed to load tours.", e);
                MessageBox.Show($"Failed to load tours: {e.Message}");
            }
        }

        private async void AddRandomTour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Info("Adding random tour.");
                await _viewModel.AddRandomTour();
                log.Info("Random tour added successfully.");
            }
            catch (Exception ex)
            {
                log.Error("Failed to add random tour.", ex);
                MessageBox.Show($"Failed to add random tour: {ex.Message}");
            }
        }
    }
}
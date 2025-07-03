using System;
using System.Windows;
using BL.Services;
using UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace UI
{
    public partial class MainWindow : Window
    {
        private readonly ITourService _tourService;
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _tourService = App.AppHost.Services.GetRequiredService<ITourService>();
            _viewModel = App.AppHost.Services.GetRequiredService<MainViewModel>();

            DataContext = _viewModel;

            LoadTours();
        }

        private async void LoadTours()
        {
            try
            {
                var tours = await _tourService.GetAllToursAsync();

                _viewModel.Tours.Clear();
                foreach (var tour in tours)
                {
                    _viewModel.Tours.Add(tour);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to load tours: {e.Message}");
            }
        }
    }
}
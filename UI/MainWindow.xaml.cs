using System.Windows;
using BL.External;
using BL.Services;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using UI.ViewModels;

namespace UI
{
    public partial class MainWindow : Window
    {
        private readonly ITourRepository _repository;
        
        public MainWindow()
        {
            InitializeComponent();
            _repository = App.AppHost.Services.GetRequiredService<ITourRepository>();
            
            var vm = App.AppHost.Services.GetRequiredService<MainViewModel>();
            DataContext = vm;
            

            LoadTours();
        }
        
        private async void LoadTours()
        {
            try
            {
                var tours = await _repository.GetAllToursAsync();
                // TODO bind ui
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }
    }
}
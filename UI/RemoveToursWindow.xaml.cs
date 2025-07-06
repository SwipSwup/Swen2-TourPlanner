using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BL.DTOs;
using BL.Services;
using log4net;
using System.Reflection;

namespace UI
{
    public partial class RemoveToursWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITourService _tourService;

        public ObservableCollection<TourDto> Tours { get; }

        public RemoveToursWindow(ObservableCollection<TourDto> tours, ITourService tourService)
        {
            InitializeComponent();

            _tourService = tourService;

            Tours = new ObservableCollection<TourDto>(tours.Select(t => new TourDto
            {
                Id = t.Id,
                Name = t.Name,
                IsSelected = false
            }));

            DataContext = this;

            log.Info($"RemoveToursWindow initialized with {Tours.Count} tours.");
        }

        private async void Ok_Click(object sender, RoutedEventArgs e)
        {
            var selectedTours = Tours.Where(t => t.IsSelected).ToList();

            if (!selectedTours.Any())
            {
                MessageBox.Show("Please select at least one tour to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                log.Warn("Ok clicked but no tours were selected for removal.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {selectedTours.Count} selected tour(s)?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                log.Info("User canceled the deletion of selected tours.");
                return;
            }

            foreach (var tour in selectedTours)
            {
                try
                {
                    await _tourService.DeleteTourAsync(tour.Id);
                    log.Info($"Deleted tour Id={tour.Id}, Name={tour.Name}");
                }
                catch (System.Exception ex)
                {
                    log.Error($"Failed to delete tour '{tour.Name}'.", ex);
                    MessageBox.Show($"Failed to delete tour '{tour.Name}': {ex.Message}");
                }
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            log.Info("RemoveToursWindow canceled by user.");
            DialogResult = false;
            Close();
        }
    }
}

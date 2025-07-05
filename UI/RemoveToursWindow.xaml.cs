using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BL.DTOs;
using BL.Services;

namespace UI
{
    public partial class RemoveToursWindow : Window
    {
        private readonly ITourService _tourService;

        public ObservableCollection<TourDto> Tours { get; }

        public RemoveToursWindow(ObservableCollection<TourDto> tours, ITourService tourService)
        {
            InitializeComponent();

            _tourService = tourService;

            // Make a copy so UI selections don’t affect original list immediately
            Tours = new ObservableCollection<TourDto>(tours.Select(t => new TourDto
            {
                Id = t.Id,
                Name = t.Name,
                IsSelected = false // Add this property in TourDto for checkbox binding
            }));

            DataContext = this;
        }

        private async void Ok_Click(object sender, RoutedEventArgs e)
        {
            var selectedTours = Tours.Where(t => t.IsSelected).ToList();

            if (!selectedTours.Any())
            {
                MessageBox.Show("Please select at least one tour to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {selectedTours.Count} selected tour(s)?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            foreach (var tour in selectedTours)
            {
                try
                {
                    await _tourService.DeleteTourAsync(tour.Id);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Failed to delete tour '{tour.Name}': {ex.Message}");
                }
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

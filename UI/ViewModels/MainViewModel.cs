using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TourPlanner.UI.Models;

namespace TourPlanner.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Tour _selectedTour;

        // ObservableCollection for holding tour data
        public ObservableCollection<Tour> Tours { get; set; }

        // Property for selected tour (used for binding)
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            try
            {
                // Adding some example tours to the ObservableCollection
                Tours = new ObservableCollection<Tour>
                {
                    new Tour { Name = "Alps Hike", Description = "A scenic hike in the Alps with breathtaking views." },
                    new Tour { Name = "Coastal Bike Ride", Description = "Ride along the coast, enjoying fresh sea breeze." },
                    new Tour { Name = "Forest Adventure", Description = "Explore deep forests and natural trails." },
                    new Tour { Name = "City Walking Tour", Description = "Discover historical places in the city." },
                    new Tour { Name = "Desert Safari", Description = "Experience an exciting off-road trip in the dunes." }
                };
            }
            catch (Exception ex)
            {
                // Error handling if something goes wrong
                MessageBox.Show($"Error initializing tours: {ex.Message}");
            }
        }

        // Implementing the INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

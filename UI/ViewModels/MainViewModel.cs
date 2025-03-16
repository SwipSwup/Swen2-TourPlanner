using System.Collections.ObjectModel;
using System.Windows.Input;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Views;

namespace TourPlanner
{
    public class MainViewModel
    {
        public ObservableCollection<Tour> Tours { get; set; }
        public ICommand AddTourCommand { get; }
        public ICommand OpenRemoveTourWindowCommand { get; }

        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set => _selectedTour = value;
        }

        public MainViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            AddTourCommand = new RelayCommand(AddTour);
            OpenRemoveTourWindowCommand = new RelayCommand(OpenRemoveTourWindow);
        }

        private void AddTour(object parameter)
        {
            var addTourWindow = new AddTourWindow();
            if (addTourWindow.ShowDialog() == true)
            {
                Tours.Add(new Tour
                {
                    Name = addTourWindow.TourName,
                    Description = addTourWindow.TourDescription,
                    From = addTourWindow.From,
                    To = addTourWindow.To,
                    TransportType = addTourWindow.TransportType,
                    Distance = addTourWindow.Distance,
                    EstimatedTime = addTourWindow.EstimatedTime,
                    ImagePath = addTourWindow.ImagePath
                });
            }
        }

        // Open Remove Tour Window
        private void OpenRemoveTourWindow(object parameter)
        {
            var removeWindow = new RemoveToursWindow(Tours);
            if (removeWindow.ShowDialog() == true)
            {
                // Update the tour list after removal
            }
        }
    }
}
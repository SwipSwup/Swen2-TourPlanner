using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Views;

namespace TourPlanner.UI.ViewModels
{
    public class AddTourViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Tour> Tours { get; set; }
        public ObservableCollection<string> TransportTypes { get; set; }
        public Tour NewTour { get; set; }
        public Tour SelectedTour { get; set; }

        // Commands
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand OpenRemoveTourWindowCommand { get; }

        public AddTourViewModel()
        {
            NewTour = new Tour();
            Tours = new ObservableCollection<Tour>();
            TransportTypes = new ObservableCollection<string> { "Car", "Bike", "Hiking", "Bus" };

            // Commands
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
            OpenRemoveTourWindowCommand = new RelayCommand(OpenRemoveTourWindow);
        }

        private void Confirm(object parameter)
        {
            ((AddTourWindow)parameter).DialogResult = true;
        }

        private void Cancel(object parameter)
        {
            ((AddTourWindow)parameter).DialogResult = false;
        }

        private void OpenRemoveTourWindow(object parameter)
        {
            var removeWindow = new RemoveToursWindow(Tours);
            if (removeWindow.ShowDialog() == true)
            {
                NotifyPropertyChanged(nameof(Tours));
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace TourPlanner.UI.Views
{
    public partial class RemoveTourLogsWindow : Window
    {
        public ObservableCollection<TourLog> TourLogs { get; set; }

        public RemoveTourLogsWindow(ObservableCollection<TourLog> tourLogs)
        {
            InitializeComponent();
            TourLogs = tourLogs;
            DataContext = this;
            TourLogListBox.ItemsSource = TourLogs;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var selectedTourLogs = TourLogs.Where(tl => tl.IsSelected).ToList();
            foreach (var tourLog in selectedTourLogs)
            {
                TourLogs.Remove(tourLog);
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
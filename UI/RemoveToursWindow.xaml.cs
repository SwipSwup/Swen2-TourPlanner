using System.Collections.ObjectModel;
using System.Windows;
using BL.Models;

namespace UI
{
    public partial class RemoveToursWindow : Window
    {
        public ObservableCollection<TourDto> Tours { get; set; }

        public RemoveToursWindow(ObservableCollection<TourDto> tours)
        {
            InitializeComponent();
            Tours = tours;
            DataContext = this;
            TourListBox.ItemsSource = Tours;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var selectedTours = Tours.Where(t => t.IsSelected).ToList();
            foreach (var tour in selectedTours)
            {
                Tours.Remove(tour);
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
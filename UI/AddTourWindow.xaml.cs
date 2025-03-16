using Microsoft.Win32;
using System.Windows;

namespace TourPlanner
{
    public partial class AddTourWindow : Window
    {
        public string TourName { get; private set; }
        public string TourDescription { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public string TransportType { get; private set; }
        public double Distance { get; private set; }
        public double EstimatedTime { get; private set; }
        public string ImagePath { get; private set; }

        public AddTourWindow()
        {
            InitializeComponent();
            TransportTypeComboBox.ItemsSource = new string[] { "Car", "Bike", "Walking", "Bus", "Train" };
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            TourName = NameTextBox.Text;
            TourDescription = DescriptionTextBox.Text;
            From = FromTextBox.Text;
            To = ToTextBox.Text;
            TransportType = TransportTypeComboBox.SelectedItem?.ToString();
            Distance = double.TryParse(DistanceTextBox.Text, out var dist) ? dist : 0;
            EstimatedTime = double.TryParse(TimeTextBox.Text, out var time) ? time : 0;
            ImagePath = ImagePathTextBox.Text;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select Route Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = openFileDialog.FileName;
            }
        }
    }
}

using System.Windows;
using BL.Models;
using Microsoft.Win32;

namespace TourPlanner
{
    public partial class EditTourWindow : Window
    {
        public Tour EditedTour { get; private set; }

        public EditTourWindow(Tour selectedTour)
        {
            InitializeComponent();
            EditedTour = selectedTour;

            // Pre-fill data in the UI elements
            NameTextBox.Text = EditedTour.Name;
            DescriptionTextBox.Text = EditedTour.Description;
            FromTextBox.Text = EditedTour.From;
            ToTextBox.Text = EditedTour.To;
            TransportTypeComboBox.SelectedItem = EditedTour.TransportType;
            DistanceTextBox.Text = EditedTour.Distance.ToString();
            TimeTextBox.Text = EditedTour.EstimatedTime.ToString();
            ImagePathTextBox.Text = EditedTour.ImagePath;

            TransportTypeComboBox.ItemsSource = new string[] { "Car", "Bike", "Walking", "Bus", "Train" };
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // Save the updated data to the EditedTour object
            EditedTour.Name = NameTextBox.Text;
            EditedTour.Description = DescriptionTextBox.Text;
            EditedTour.From = FromTextBox.Text;
            EditedTour.To = ToTextBox.Text;
            EditedTour.TransportType = TransportTypeComboBox.SelectedItem?.ToString();
            EditedTour.Distance = double.TryParse(DistanceTextBox.Text, out var dist) ? dist : 0;
            EditedTour.EstimatedTime = double.TryParse(TimeTextBox.Text, out var time) ? time : 0;
            EditedTour.ImagePath = ImagePathTextBox.Text;

            // Validate the edited Tour before closing the window
            var errors = EditedTour.Validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Prevent closing the window if validation fails
            }

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

using System;
using System.Windows;
using BL.DTOs;
using Microsoft.Win32;
using log4net;
using System.Reflection;

namespace UI
{
    public partial class EditTourWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TourDto EditedTour { get; private set; }

        public EditTourWindow(TourDto selectedTour)
        {
            InitializeComponent();

            log.Info("Initializing EditTourWindow.");

            EditedTour = new TourDto
            {
                Id = selectedTour.Id,
                Name = selectedTour.Name,
                Description = selectedTour.Description,
                From = selectedTour.From,
                To = selectedTour.To,
                TransportType = selectedTour.TransportType,
                Distance = selectedTour.Distance,
                EstimatedTime = selectedTour.EstimatedTime,
                ImagePath = selectedTour.ImagePath,
                TourLogs = selectedTour.TourLogs
            };

            // Pre-fill fields
            NameTextBox.Text = EditedTour.Name;
            DescriptionTextBox.Text = EditedTour.Description;
            FromTextBox.Text = EditedTour.From;
            ToTextBox.Text = EditedTour.To;
            TransportTypeComboBox.ItemsSource = new string[] { "Car", "Bike", "Foot", "Train" };
            TransportTypeComboBox.SelectedItem = EditedTour.TransportType;

            log.Info($"Loaded tour for editing: Id={EditedTour.Id}, Name={EditedTour.Name}");
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            EditedTour.Name = NameTextBox.Text;
            EditedTour.Description = DescriptionTextBox.Text;
            EditedTour.From = FromTextBox.Text;
            EditedTour.To = ToTextBox.Text;
            EditedTour.TransportType = TransportTypeComboBox.SelectedItem?.ToString();

            var errors = EditedTour.Validate();

            if (errors.Count > 0)
            {
                string errorMessage = string.Join("\n", errors);
                log.Warn($"Validation failed on Confirm: {errorMessage}");
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            log.Info($"Tour confirmed for saving: Id={EditedTour.Id}, Name={EditedTour.Name}");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            log.Info("EditTourWindow canceled by user.");
            DialogResult = false;
            Close();
        }
    }
}

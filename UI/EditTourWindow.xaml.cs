﻿using System;
using System.Windows;
using BL.DTOs;
using Microsoft.Win32;

namespace UI
{
    public partial class EditTourWindow : Window
    {
        public TourDto EditedTour { get; private set; }

        public EditTourWindow(TourDto selectedTour)
        {
            InitializeComponent();

            // Make a **copy** to avoid direct editing of the original until confirmed
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
            DistanceTextBox.Text = EditedTour.Distance.ToString();
            TimeTextBox.Text = EditedTour.EstimatedTime.ToString();
            ImagePathTextBox.Text = EditedTour.ImagePath;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            EditedTour.Name = NameTextBox.Text;
            EditedTour.Description = DescriptionTextBox.Text;
            EditedTour.From = FromTextBox.Text;
            EditedTour.To = ToTextBox.Text;
            EditedTour.TransportType = TransportTypeComboBox.SelectedItem?.ToString();
            EditedTour.Distance = float.TryParse(DistanceTextBox.Text, out var dist) ? dist : 0;
            EditedTour.EstimatedTime = TimeSpan.TryParse(TimeTextBox.Text, out var time) ? time : TimeSpan.Zero;
            EditedTour.ImagePath = ImagePathTextBox.Text;

            var errors = EditedTour.Validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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

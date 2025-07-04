﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace UI
{
    public partial class AddTourLogWindow : Window
    {
        public DateTime Date { get; private set; }
        public TimeSpan Duration { get; private set; }
        public float TotalDistance { get; private set; }
        public string Comment { get; private set; }
        public int Difficulty { get; private set; }
        public int Rating { get; private set; }

        public AddTourLogWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var errors = new List<string>();

            if (!DateTime.TryParse(DateTextBox.Text, out var date))
                errors.Add("Invalid date format.");

            if (!TimeSpan.TryParse(DurationTextBox.Text, out var duration))
                errors.Add("Invalid duration format.");

            if (!float.TryParse(DistanceTextBox.Text, out var distance))
                errors.Add("Invalid distance.");

            var comment = CommentTextBox.Text;

            if (!int.TryParse(DifficultyTextBox.Text, out var difficulty) || difficulty < 1 || difficulty > 5)
                errors.Add("Difficulty must be an integer between 1 and 5.");

            if (!int.TryParse(RatingTextBox.Text, out var rating) || rating < 1 || rating > 5)
                errors.Add("Rating must be an integer between 1 and 5.");

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Date = date;
            Duration = duration;
            TotalDistance = distance;
            Comment = comment;
            Difficulty = difficulty;
            Rating = rating;

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

using System;
using System.Collections.Generic;
using System.Windows;
using BL.DTOs;
using log4net;

namespace UI
{
    public partial class EditTourLogWindow : Window
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(EditTourLogWindow));

        public DateTime Date { get; private set; }
        public TimeSpan Duration { get; private set; }
        public float TotalDistance { get; private set; }
        public string Comment { get; private set; }
        public int Difficulty { get; private set; }
        public int Rating { get; private set; }

        public EditTourLogWindow(TourLogDto tourLog)
        {
            InitializeComponent();

            _log.Info("EditTourLogWindow opened for editing.");

            // Pre-fill fields with existing data
            DateTextBox.Text = tourLog.DateTime.ToString("yyyy-MM-dd");
            DurationTextBox.Text = tourLog.TotalTime.ToString();
            DistanceTextBox.Text = tourLog.TotalDistance.ToString();
            CommentTextBox.Text = tourLog.Comment;
            DifficultyTextBox.Text = tourLog.Difficulty.ToString();
            RatingTextBox.Text = tourLog.Rating.ToString();
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
                _log.Warn("Validation errors in EditTourLogWindow:\n" + string.Join("\n", errors));
                MessageBox.Show(string.Join("\n", errors), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            Duration = duration;
            TotalDistance = distance;
            Comment = comment;
            Difficulty = difficulty;
            Rating = rating;

            _log.Info("Tour log edit confirmed by user.");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _log.Info("Tour log edit canceled by user.");
            DialogResult = false;
            Close();
        }
    }
}

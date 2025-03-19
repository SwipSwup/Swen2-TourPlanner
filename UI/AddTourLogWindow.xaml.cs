using System.Windows;

namespace UI
{


    public partial class AddTourLogWindow : Window
    {
        public string Date { get; private set; }
        public string Duration { get; private set; }
        public string Distance { get; private set; }

        public AddTourLogWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Date = DateTextBox.Text;
            Duration = DurationTextBox.Text;
            Distance = DistanceTextBox.Text;

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
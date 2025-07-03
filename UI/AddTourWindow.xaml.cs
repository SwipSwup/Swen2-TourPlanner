using System.Windows;
using Microsoft.Win32;

namespace UI
{
    public partial class AddTourWindow : Window
    {
        public string TourName => NameTextBox.Text;
        public string TourDescription => DescriptionTextBox.Text;
        public string From => FromTextBox.Text;
        public string To => ToTextBox.Text;
        public float Distance => float.Parse(DistanceTextBox.Text);
        public TimeSpan EstimatedTime => TimeSpan.Parse(TimeTextBox.Text);
        public string TransportType => TransportTypeComboBox.Text;


        public AddTourWindow()
        {
            InitializeComponent();

            // Fill TransportType combo box
            TransportTypeComboBox.ItemsSource = new[] { "Car", "Bike", "Foot", "Train" };
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {


            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


    }
}
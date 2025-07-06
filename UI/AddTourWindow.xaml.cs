using System.Windows;
using Microsoft.Win32;
using log4net;

namespace UI
{
    public partial class AddTourWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AddTourWindow));

        public string TourName => NameTextBox.Text;
        public string TourDescription => DescriptionTextBox.Text;
        public string From => FromTextBox.Text;
        public string To => ToTextBox.Text;
        public string TransportType => TransportTypeComboBox.Text;

        public AddTourWindow()
        {
            InitializeComponent();

            // Fill TransportType combo box
            TransportTypeComboBox.ItemsSource = new[] { "Car", "Bike", "Foot", "Train" };

            Log.Info("AddTourWindow initialized.");
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Log.Info($"Tour confirmed: Name={TourName}, From={From}, To={To}, Transport={TransportType}");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Tour creation canceled.");
            DialogResult = false;
            Close();
        }
    }
}
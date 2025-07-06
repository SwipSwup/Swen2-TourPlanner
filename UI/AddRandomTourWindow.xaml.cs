using System.Windows;
using System.Windows.Controls;
using log4net;

namespace UI.Views
{
    public partial class AddRandomTourWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AddRandomTourWindow));

        public string From { get; private set; }
        public string TransportType { get; private set; }
        public string UserDescription { get; private set; }

        public AddRandomTourWindow()
        {
            InitializeComponent();
            Log.Info("AddRandomTourWindow initialized.");
        }

        private void AddRandomTour_Click(object sender, RoutedEventArgs e)
        {
            From = FromTextBox.Text;
            if (TransportComboBox.SelectedItem is ComboBoxItem item)
                TransportType = item.Content.ToString();

            if (string.IsNullOrWhiteSpace(From) || string.IsNullOrWhiteSpace(TransportType))
            {
                Log.Warn("AddRandomTour clicked with missing input.");
                MessageBox.Show("Please enter a start location and select a transport type.");
                return;
            }

            Log.Info($"Random tour confirmed. From: {From}, Transport: {TransportType}");
            DialogResult = true;
            Close();
        }
    }
}
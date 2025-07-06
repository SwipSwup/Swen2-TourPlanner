using System.Windows;
using System.Windows.Controls;

namespace UI.Views
{
    public partial class AddRandomTourWindow : Window
    {
        public string From { get; private set; }
        public string TransportType { get; private set; }
        public string UserDescription { get; private set; }

        public AddRandomTourWindow()
        {
            InitializeComponent();
        }

        private void AddRandomTour_Click(object sender, RoutedEventArgs e)
        {
            From = FromTextBox.Text;
            if (TransportComboBox.SelectedItem is ComboBoxItem item)
                TransportType = item.Content.ToString();

            if (string.IsNullOrWhiteSpace(From) || string.IsNullOrWhiteSpace(TransportType))
            {
                MessageBox.Show("Please enter a start location and select a transport type.");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
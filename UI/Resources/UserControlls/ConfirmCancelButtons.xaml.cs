using System.Windows;
using System.Windows.Controls;

namespace TourPlanner.UserControls
{
    public partial class ConfirmCancelButtons : UserControl
    {
        public ConfirmCancelButtons()
        {
            InitializeComponent();
        }

        // Event handlers that you will raise (forward) to the parent
        public event RoutedEventHandler OkButtonClicked;
        public event RoutedEventHandler CancelButtonClicked;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise event to parent
            OkButtonClicked?.Invoke(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise event to parent
            CancelButtonClicked?.Invoke(this, e);
        }
    }
}
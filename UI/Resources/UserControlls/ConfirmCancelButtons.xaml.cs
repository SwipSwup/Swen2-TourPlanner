using System.Windows;
using System.Windows.Controls;

namespace UI.Resources.UserControlls
{
    public partial class ConfirmCancelButtons : UserControl
    {
        public ConfirmCancelButtons()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler OkButtonClicked;
        public event RoutedEventHandler CancelButtonClicked;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            OkButtonClicked?.Invoke(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelButtonClicked?.Invoke(this, e);
        }
    }
}
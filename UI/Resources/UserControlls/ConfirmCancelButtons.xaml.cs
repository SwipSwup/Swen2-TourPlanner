using System.Windows;
using System.Windows.Controls;
using log4net;

namespace UI.Resources.UserControlls
{
    public partial class ConfirmCancelButtons : UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ConfirmCancelButtons));

        public ConfirmCancelButtons()
        {
            InitializeComponent();
            _log.Info("ConfirmCancelButtons initialized.");
        }

        public event RoutedEventHandler OkButtonClicked;
        public event RoutedEventHandler CancelButtonClicked;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _log.Info("OK button clicked.");
            OkButtonClicked?.Invoke(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _log.Info("Cancel button clicked.");
            CancelButtonClicked?.Invoke(this, e);
        }
    }
}
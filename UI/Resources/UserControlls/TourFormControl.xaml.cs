using System.Windows.Controls;
using log4net;

namespace UI.Resources.UserControlls
{
    public partial class TourFormControl : UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TourFormControl));

        public TourFormControl()
        {
            InitializeComponent();
            _log.Info("TourFormControl initialized.");
        }
    }
}
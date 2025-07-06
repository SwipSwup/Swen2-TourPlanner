using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BL.DTOs;
using log4net;
using System.Reflection;

namespace UI
{
    public partial class RemoveTourLogsWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ObservableCollection<TourLogDto> TourLogs { get; set; }

        public RemoveTourLogsWindow(ObservableCollection<TourLogDto> tourLogs)
        {
            InitializeComponent();
            TourLogs = tourLogs;
            DataContext = this;
            TourLogListBox.ItemsSource = TourLogs;

            log.Info($"RemoveTourLogsWindow initialized with {TourLogs.Count} tour logs.");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var selectedTourLogs = TourLogs.Where(tl => tl.IsSelected).ToList();
            log.Info($"Ok clicked. Selected {selectedTourLogs.Count} tour logs for removal.");

            foreach (var tourLog in selectedTourLogs)
            {
                TourLogs.Remove(tourLog);
                log.Info($"Removed tour log Id={tourLog.Id}");
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            log.Info("RemoveTourLogsWindow canceled by user.");
            DialogResult = false;
            Close();
        }
    }
}
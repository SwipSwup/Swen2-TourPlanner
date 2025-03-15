using System.Collections.ObjectModel;

namespace TourPlanner
{
    public class Tour
    {
        public string Name { get; set; }
    }

    public class TourLog
    {
        public string Date { get; set; }
        public string Duration { get; set; }
        public string Distance { get; set; }
    }

    public class MainViewModel
    {
        public ObservableCollection<Tour> Tours { get; set; }
        public ObservableCollection<TourLog> TourLogs { get; set; }

        public MainViewModel()
        {
            Tours = new ObservableCollection<Tour>
            {
                new Tour { Name = "Wienerwald" },
                new Tour { Name = "Dopplerhütte" },
                new Tour { Name = "Figlwarte" },
                new Tour { Name = "Dorfrunde" }
            };

            TourLogs = new ObservableCollection<TourLog>
            {
                new TourLog { Date = "Value 1", Duration = "Value 2", Distance = "Value 3" },
                new TourLog { Date = "Value 4", Duration = "Value 5", Distance = "Value 6" },
                new TourLog { Date = "Value 7", Duration = "Value 8", Distance = "Value 9" },
                new TourLog { Date = "Value 10", Duration = "Value 11", Distance = "Value 12" }
            };
        }
    }
}
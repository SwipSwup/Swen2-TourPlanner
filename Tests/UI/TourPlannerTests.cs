using BL.Models;

namespace Tests
{
    [TestFixture]
    public class TourPlannerTests
    {
        [Test]
        public void Tour_Validate_ValidTour_ReturnsTrue()
        {
            var tour = new Tour
            {
                Name = "Mountain Trip",
                Description = "A wonderful mountain trip",
                From = "Vienna",
                To = "Salzburg",
                TransportType = "Car",
                Distance = 300,
                EstimatedTime = 5
            };

            var validationErrors = tour.Validate();

            Assert.IsEmpty(validationErrors);
        }



        [Test]
        public void Tour_Validate_InvalidTour_ReturnsFalse()
        {
            var tour = new Tour
            {
                Name = "",
                Description = "Short description",
                From = "Vienna",
                To = "Salzburg",
                TransportType = "Car",
                Distance = 300,
                EstimatedTime = 5
            };

            var validationErrors = tour.Validate();

            Assert.IsNotEmpty(validationErrors);
        }

        [Test]
        public void TourLog_Validate_ValidTourLog_ReturnsTrue()
        {
            var tourLog = new TourLog
            {
                Date = "15.03.2025",
                Duration = "3",
                Distance = "120"
            };

            var validationErrors = tourLog.Validate();

            Assert.IsEmpty(validationErrors);
        }

        [Test]
        public void TourLog_Validate_InvalidTourLog_ReturnsFalse()
        {
            var tourLog = new TourLog
            {
                Date = "invalid date", 
                Duration = "0", 
                Distance = "100"
            };

            var validationErrors = tourLog.Validate();

            Assert.IsNotEmpty(validationErrors);
        }
    }
}

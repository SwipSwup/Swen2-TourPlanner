using BL.Models;

namespace Tests
{
    [TestFixture]
    public class TourPlannerTests
    {
        // 1. **Tour Validation Test**
        [Test]
        public void Tour_Validate_ValidTour_ReturnsTrue()
        {
            // Arrange
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

            // Act
            var validationErrors = tour.Validate();

            // Assert
            Assert.IsEmpty(validationErrors);
        }



        [Test]
        public void Tour_Validate_InvalidTour_ReturnsFalse()
        {
            // Arrange
            var tour = new Tour
            {
                Name = "", // Invalid Name
                Description = "Short description",
                From = "Vienna",
                To = "Salzburg",
                TransportType = "Car",
                Distance = 300,
                EstimatedTime = 5
            };

            // Act
            var validationErrors = tour.Validate();

            // Assert
            Assert.IsNotEmpty(validationErrors);
        }

        // 2. **TourLog Validation Test**
        [Test]
        public void TourLog_Validate_ValidTourLog_ReturnsTrue()
        {
            // Arrange
            var tourLog = new TourLog
            {
                Date = "15.03.2025",
                Duration = "3",
                Distance = "120"
            };

            // Act
            var validationErrors = tourLog.Validate();

            // Assert
            Assert.IsEmpty(validationErrors);
        }

        [Test]
        public void TourLog_Validate_InvalidTourLog_ReturnsFalse()
        {
            // Arrange
            var tourLog = new TourLog
            {
                Date = "invalid date", // Invalid Date
                Duration = "0", // Invalid Duration
                Distance = "100"
            };

            // Act
            var validationErrors = tourLog.Validate();

            // Assert
            Assert.IsNotEmpty(validationErrors);
        }

        // 3. **Test for Adding a Tour**
        [Test]
        public void AddTour_ValidTour_AddsTourToList()
        {
            // Arrange
            var tourManager = new TourManager(); // Assuming you have a TourManager class to manage adding tours
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

            // Act
            tourManager.AddTour(tour);
            var addedTour = tourManager.GetTourByName("Mountain Trip");

            // Assert
            Assert.AreEqual(tour.Name, addedTour.Name);
        }
    }
}

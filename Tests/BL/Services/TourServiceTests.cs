using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BL.DTOs;
using BL.External;
using BL.Services;
using BL.TourImage;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace Tests.BL.Services;

[TestFixture]
public class TourServiceTests
{
    private TourService _tourService = null!;
    private Mock<ITourRepository> _mockRepo = null!;
    private Mock<IRouteService> _mockRouteService = null!;
    private Mock<IMapImageGenerator> _mockImageGenerator = null!;
    private IConfiguration _config = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITourRepository>();
        _mockRouteService = new Mock<IRouteService>();
        _mockImageGenerator = new Mock<IMapImageGenerator>();

        var settings = new Dictionary<string, string>();
        _config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        _tourService = new TourService(_config, _mockRepo.Object, _mockRouteService.Object, _mockImageGenerator.Object);
    }

    [Test]
    public async Task UpdateTourAsync_UpdatesTourSuccessfully()
    {
        // Arrange
        var dto = new TourDto
        {
            Id = 5,
            Name = "Updated Tour",
            Description = "Updated description",
            From = "CityA",
            To = "CityB",
            TransportType = "Car"
        };

        var routeResult = new RouteResult
        {
            Distance = 100,
            EstimatedTime = TimeSpan.FromHours(1.5)
        };

        _mockRouteService.Setup(s => s.GetRouteAsync(dto.From, dto.To))
                         .ReturnsAsync(routeResult);

        _mockImageGenerator.Setup(g => g.GenerateMapImageWithLeaflet(It.IsAny<RouteResult>()))
                           .ReturnsAsync("new/image/path.png");

        // Act
        await _tourService.UpdateTourAsync(dto);

        // Assert
        _mockRepo.Verify(r => r.UpdateTourAsync(It.Is<Tour>(t =>
            t.Id == dto.Id &&
            t.Name == dto.Name &&
            t.Distance == routeResult.Distance &&
            t.EstimatedTime == routeResult.EstimatedTime &&
            t.ImagePath == "new/image/path.png"
        )), Times.Once);
    }

    [Test]
    public async Task UpdateTourLogAsync_UpdatesLogSuccessfully()
    {
        // Arrange
        var logDto = new TourLogDto
        {
            Id = 10,
            Comment = "Updated comment",
            DateTime = DateTime.Now,
            Difficulty = 3,
            TotalDistance = 20,
            TotalTime = TimeSpan.FromHours(1),
            Rating = 4
        };

        // Act
        await _tourService.UpdateTourLogAsync(logDto);

        // Assert
        _mockRepo.Verify(r => r.UpdateTourLogAsync(It.Is<TourLog>(l =>
            l.Id == logDto.Id &&
            l.Comment == logDto.Comment &&
            l.Rating == logDto.Rating
        )), Times.Once);
    }

    [Test]
    public async Task DeleteTourLogAsync_DeletesLogSuccessfully()
    {
        // Act
        await _tourService.DeleteTourLogAsync(7);

        // Assert
        _mockRepo.Verify(r => r.DeleteTourLogAsync(7), Times.Once);
    }

    [Test]
    public void GetAllToursAsync_ThrowsException_LogsError()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllToursAsync()).ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _tourService.GetAllToursAsync());
        Assert.That(ex.Message, Is.EqualTo("DB error"));

        // Verify repo was called
        _mockRepo.Verify(r => r.GetAllToursAsync(), Times.Once);
    }

    [Test]
    public async Task ImportToursFromJsonAsync_ImportsToursSuccessfully()
    {
        // Arrange
        var tours = new List<TourDto>
        {
            new() { Name = "Tour1", From = "A", To = "B", TransportType = "Car" },
            new() { Name = "Tour2", From = "C", To = "D", TransportType = "Bike" }
        };

        string tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, System.Text.Json.JsonSerializer.Serialize(tours));

        var createTourCalls = 0;
        _mockRouteService.Setup(s => s.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(new RouteResult { Distance = 10, EstimatedTime = TimeSpan.FromMinutes(30) });
        _mockImageGenerator.Setup(g => g.GenerateMapImageWithLeaflet(It.IsAny<RouteResult>()))
                           .ReturnsAsync("img.png");
        _mockRepo.Setup(r => r.AddTourAsync(It.IsAny<Tour>())).Returns(Task.CompletedTask)
                 .Callback(() => createTourCalls++);

        // Act
        await _tourService.ImportToursFromJsonAsync(tempFile);

        // Assert
        Assert.That(createTourCalls, Is.EqualTo(tours.Count));

        // Cleanup
        File.Delete(tempFile);
    }

    [Test]
    public void ImportToursFromJsonAsync_InvalidFile_ThrowsException()
    {
        // Arrange
        string invalidPath = "nonexistentfile.json";

        // Act & Assert
        Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await _tourService.ImportToursFromJsonAsync(invalidPath));
    }

    [Test]
    public async Task CreateTourAsync_CreatesTourSuccessfully()
    {
        // Arrange
        var dto = new TourDto
        {
            Name = "New Tour",
            Description = "Description",
            From = "Start",
            To = "End",
            TransportType = "Bike"
        };

        var routeResult = new RouteResult
        {
            Distance = 50,
            EstimatedTime = TimeSpan.FromHours(2)
        };

        _mockRouteService.Setup(s => s.GetRouteAsync(dto.From, dto.To))
            .ReturnsAsync(routeResult);

        _mockImageGenerator.Setup(g => g.GenerateMapImageWithLeaflet(routeResult))
            .ReturnsAsync("image/path.png");

        // Act
        await _tourService.CreateTourAsync(dto);

        // Assert
        _mockRepo.Verify(r => r.AddTourAsync(It.Is<Tour>(t =>
            t.Name == dto.Name &&
            t.Distance == routeResult.Distance &&
            t.EstimatedTime == routeResult.EstimatedTime &&
            t.ImagePath == "image/path.png"
        )), Times.Once);
    }

    [Test]
    public async Task GetLogsForTourAsync_ReturnsTourLogs()
    {
        // Arrange
        int tourId = 42;
        var logs = new List<TourLog>
        {
            new TourLog { Id = 1, Comment = "Great tour", Rating = 5 },
            new TourLog { Id = 2, Comment = "Nice view", Rating = 4 }
        };

        _mockRepo.Setup(r => r.GetLogsForTourAsync(tourId))
            .ReturnsAsync(logs);

        // Act
        var result = await _tourService.GetLogsForTourAsync(tourId);

        // Assert
        Assert.That(result.Count, Is.EqualTo(logs.Count));
        Assert.That(result.Any(l => l.Comment == "Great tour" && l.Rating == 5));
        Assert.That(result.Any(l => l.Comment == "Nice view" && l.Rating == 4));
    }

}

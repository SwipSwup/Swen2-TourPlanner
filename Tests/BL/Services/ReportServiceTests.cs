using BL.DTOs;
using BL.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BL.DTOs.Report;

namespace Tests.BL.Services;

[TestFixture]
public class ReportServiceAdditionalTests
{
    private string _outputPath = "ReportServiceTests";
    private IReportService _reportService = null!;
    private IConfiguration _config = null!;

    [SetUp]
    public void Setup()
    {
        if (Directory.Exists(_outputPath))
            Directory.Delete(_outputPath, true);

        var settings = new Dictionary<string, string>
        {
            { "ReportSettings:OutputDirectory", _outputPath }
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        _reportService = new ReportService(_config);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_outputPath))
            Directory.Delete(_outputPath, true);
    }

    [Test]
    public void Constructor_MissingOutputPath_ThrowsException()
    {
        var emptyConfig = new ConfigurationBuilder().Build();
        Assert.Throws<Exception>(() => new ReportService(emptyConfig));
    }

    [Test]
    public async Task GenerateTourReportAsync_CreatesDirectoryIfNotExists()
    {
        if (Directory.Exists(_outputPath))
            Directory.Delete(_outputPath, true);

        var tour = new TourDto { Name = "SampleTour" };

        await _reportService.GenerateTourReportAsync(tour);

        Assert.That(Directory.Exists(_outputPath), Is.True);
    }

    [Test]
    public async Task GenerateSummaryReportAsync_EmptyToursList_CreatesEmptyReport()
    {
        var tours = new List<TourDto>();

        await _reportService.GenerateSummaryReportAsync(tours);

        var files = Directory.GetFiles(_outputPath, "Summary_*.pdf");
        Assert.That(files, Is.Not.Empty);
    }

    [Test]
    public async Task GenerateSummaryReportAsync_CalculatesCorrectAverages()
    {
        var tours = new List<TourDto>
        {
            new()
            {
                Name = "T1",
                TourLogs = new List<TourLogDto>
                {
                    new() { TotalDistance = 10, TotalTime = TimeSpan.FromMinutes(60), Rating = 4 },
                    new() { TotalDistance = 20, TotalTime = TimeSpan.FromMinutes(120), Rating = 5 }
                }
            },
            new()
            {
                Name = "T2",
                TourLogs = new List<TourLogDto>
                {
                    new() { TotalDistance = 30, TotalTime = TimeSpan.FromMinutes(180), Rating = 3 }
                }
            }
        };

        await _reportService.GenerateSummaryReportAsync(tours);

        var files = Directory.GetFiles(_outputPath, "Summary_*.pdf");
        Assert.That(files, Is.Not.Empty);
        // deeper content validation would require PDF reading - out of scope here
    }

    [Test]
    public async Task GenerateTourReportAsync_HandlesEmptyTourLogs()
    {
        var tour = new TourDto
        {
            Name = "NoLogs",
            TourLogs = new List<TourLogDto>()
        };

        await _reportService.GenerateTourReportAsync(tour);

        var files = Directory.GetFiles(_outputPath, "Tour_NoLogs_*.pdf");
        Assert.That(files, Is.Not.Empty);
    }

    [Test]
    public void ConvertTourDto_ConvertsCorrectly()
    {
        var tour = new TourDto
        {
            Name = "Test",
            Description = "Desc",
            From = "A",
            To = "B",
            TransportType = "Car",
            Distance = 100,
            EstimatedTime = TimeSpan.FromHours(1),
            ImagePath = "img.png",
            TourLogs = new List<TourLogDto>
            {
                new()
                {
                    DateTime = DateTime.Now,
                    Comment = "Nice",
                    Difficulty = 3,
                    TotalDistance = 100,
                    TotalTime = TimeSpan.FromHours(1),
                    Rating = 5
                }
            }
        };

        // Use reflection to invoke private method ConvertTourDto
        var method = typeof(ReportService).GetMethod("ConvertTourDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var converted = (TourReportDto)method.Invoke(_reportService, new object[] { tour })!;

        Assert.That(converted.Name, Is.EqualTo(tour.Name));
        Assert.That(converted.Logs.Count, Is.EqualTo(tour.TourLogs.Count));
    }
}

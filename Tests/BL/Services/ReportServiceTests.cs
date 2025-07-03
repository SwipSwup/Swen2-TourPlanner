using BL.DTOs;
using BL.Services;
using Microsoft.Extensions.Configuration;

namespace Tests.BL.Services;

[TestFixture]
public class ReportServiceTests
{
    private string _outputPath = null!;
    private IReportService _service = null!;

    [SetUp]
    public void Setup()
    {
        _outputPath = Path.Combine("TourPlannerReportTests");

        Dictionary<string, string> settings = new Dictionary<string, string>
        {
            { "ReportSettings:OutputDirectory", _outputPath }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        _service = new ReportService(config);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_outputPath))
        {
            foreach (var file in Directory.GetFiles(_outputPath, "*.pdf"))
            {
                File.Delete(file);
            }
        }
    }

    [Test]
    public async Task GenerateTourReport_CreatesPdfFile()
    {
        var tour = new TourDto
        {
            Name = "TestTour",
            Description = "Testing tour generation",
            From = "Vienna",
            To = "Salzburg",
            TransportType = "Train",
            Distance = 300,
            EstimatedTime = TimeSpan.FromHours(3),
            ImagePath = "", // Skip image
            TourLogs = new List<TourLogDto>
            {
                new()
                {
                    DateTime = DateTime.Now,
                    Comment = "Great tour!",
                    Difficulty = 1,
                    TotalDistance = 300,
                    TotalTime = TimeSpan.FromHours(3),
                    Rating = 5
                }
            }
        };

        await _service.GenerateTourReportAsync(tour);

        string[] files = Directory.GetFiles(_outputPath, "Tour_TestTour_*.pdf");
        
        Assert.That(files, Is.Not.Empty);
    }

    [Test]
    public async Task GenerateSummaryReport_CreatesPdfFile()
    {
        var tours = new List<TourDto>
        {
            new()
            {
                Name = "Tour A",
                TourLogs = new List<TourLogDto>
                {
                    new() { TotalDistance = 10, TotalTime = TimeSpan.FromMinutes(60), Rating = 4 },
                    new() { TotalDistance = 15, TotalTime = TimeSpan.FromMinutes(90), Rating = 5 }
                }
            },
            new()
            {
                Name = "Tour B",
                TourLogs = new List<TourLogDto>()
            }
        };

        await _service.GenerateSummaryReportAsync(tours);

        string[] files = Directory.GetFiles(_outputPath, "Summary_*.pdf");
        Assert.That(files, Is.Not.Empty);
    }
}

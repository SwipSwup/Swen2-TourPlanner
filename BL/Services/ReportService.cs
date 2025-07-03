using BL.DTOs;
using BL.DTOs.Report;
using BL.Reports;
using Microsoft.Extensions.Configuration;

namespace BL.Services;

public class ReportService(IConfiguration config) : IReportService
{
    private readonly string _outputPath = config["ReportSettings:OutputDirectory"]
                                          ?? throw new Exception("Missing ReportSettings:OutputDirectory");

    public Task GenerateTourReportAsync(TourDto tour)
    {
        string fileName = Path.Combine(_outputPath, $"Tour_{tour.Name}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        Directory.CreateDirectory(_outputPath);
        TourReportGenerator.Generate(ConvertTourDto(tour), fileName);
        return Task.CompletedTask;
    }

    public Task GenerateSummaryReportAsync(List<TourDto> tours)
    {
        string fileName = Path.Combine(_outputPath, $"Summary_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        Directory.CreateDirectory(_outputPath);

        var entries = tours.Select(t => new SummaryEntryDto
        {
            TourName = t.Name,
            AvgDistance = t.TourLogs.Any() ? t.TourLogs.Average(l => l.TotalDistance) : 0,
            AvgTime = t.TourLogs.Any()
                ? TimeSpan.FromSeconds(t.TourLogs.Average(l => l.TotalTime.TotalSeconds))
                : TimeSpan.Zero,
            AvgRating = t.TourLogs.Any() ? t.TourLogs.Average(l => l.Rating) : 0
        }).ToList();

        SummaryReportGenerator.Generate(entries, fileName);
        return Task.CompletedTask;
    }

    private TourReportDto ConvertTourDto(TourDto t) => new()
    {
        Name = t.Name,
        Description = t.Description,
        From = t.From,
        To = t.To,
        TransportType = t.TransportType,
        Distance = t.Distance,
        EstimatedTime = t.EstimatedTime,
        ImagePath = t.ImagePath,
        Logs = t.TourLogs.Select(l => new TourLogDto
        {
            DateTime = l.DateTime,
            Comment = l.Comment,
            Difficulty = l.Difficulty,
            TotalDistance = l.TotalDistance,
            TotalTime = l.TotalTime,
            Rating = l.Rating
        }).ToList()
    };
}

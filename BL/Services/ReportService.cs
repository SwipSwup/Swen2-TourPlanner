using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BL.DTOs;
using BL.DTOs.Report;
using BL.Reports;
using Microsoft.Extensions.Configuration;
using QuestPDF.Infrastructure;
using log4net;
using System.Reflection;

namespace BL.Services;

public class ReportService(IConfiguration config) : IReportService
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly string _outputPath = config["ReportSettings:OutputDirectory"]
                                          ?? throw new Exception("Missing ReportSettings:OutputDirectory");

    public Task GenerateTourReportAsync(TourDto tour)
    {
        try
        {
            log.Info($"Generating tour report for '{tour.Name}'.");

            QuestPDF.Settings.License = LicenseType.Community;

            string dirPath = Path.Combine(AppContext.BaseDirectory, _outputPath);
            Directory.CreateDirectory(dirPath);

            string fileName = Path.Combine(dirPath, $"Tour_{tour.Name}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            TourReportGenerator.Generate(ConvertTourDto(tour), fileName);

            log.Info($"Tour report for '{tour.Name}' generated successfully at '{fileName}'.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to generate tour report for '{tour.Name}'.", ex);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task GenerateSummaryReportAsync(List<TourDto> tours)
    {
        try
        {
            log.Info("Generating summary report.");

            QuestPDF.Settings.License = LicenseType.Community;

            string dirPath = Path.Combine(AppContext.BaseDirectory, _outputPath);
            Directory.CreateDirectory(dirPath);

            string fileName = Path.Combine(dirPath, $"Summary_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            List<SummaryEntryDto> entries = tours.Select(t => new SummaryEntryDto
            {
                TourName = t.Name,
                AvgDistance = t.TourLogs.Any() ? t.TourLogs.Average(l => l.TotalDistance) : 0,
                AvgTime = t.TourLogs.Any()
                    ? TimeSpan.FromSeconds(t.TourLogs.Average(l => l.TotalTime.TotalSeconds))
                    : TimeSpan.Zero,
                AvgRating = t.TourLogs.Any() ? t.TourLogs.Average(l => l.Rating) : 0
            }).ToList();

            SummaryReportGenerator.Generate(entries, fileName);

            log.Info($"Summary report generated successfully at '{fileName}'.");
        }
        catch (Exception ex)
        {
            log.Error("Failed to generate summary report.", ex);
            throw;
        }

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

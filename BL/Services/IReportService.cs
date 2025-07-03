using BL.DTOs;

namespace BL.Services;

public interface IReportService
{
    Task GenerateTourReportAsync(TourDto tour);
    Task GenerateSummaryReportAsync(List<TourDto> tours);
}

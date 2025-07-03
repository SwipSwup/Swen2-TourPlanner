namespace BL.DTOs.Report;

public class TourReportDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public float Distance { get; set; }
    public TimeSpan EstimatedTime { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public List<TourLogDto> Logs { get; set; } = new();
}
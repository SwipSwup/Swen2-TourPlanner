namespace BL.DTOs;

public class SummaryEntryDto
{
    public string TourName { get; set; } = string.Empty;
    public float AvgDistance { get; set; }
    public TimeSpan AvgTime { get; set; }
    public double AvgRating { get; set; }
}
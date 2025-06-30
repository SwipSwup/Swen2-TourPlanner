namespace DAL.Models;

public class Tour
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public float Distance { get; set; }
    public TimeSpan EstimatedTime { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    public ICollection<TourLog> TourLogs { get; set; } = new List<TourLog>();
}
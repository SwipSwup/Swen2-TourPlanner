namespace DAL.Models;

public class TourLog
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public float TotalDistance { get; set; }
    public TimeSpan TotalTime { get; set; }
    public int Rating { get; set; }

    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;
}
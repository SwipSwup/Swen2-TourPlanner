namespace BL.External;

public class RouteResult
{
    public float Distance { get; set; }
    public TimeSpan EstimatedTime { get; set; }
    public string ImagePath { get; set; } = string.Empty;
}
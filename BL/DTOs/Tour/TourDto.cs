namespace BL.DTOs;

public class TourDto
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
    public List<TourLogDto> TourLogs { get; set; } = new();
    
    
    public int Popularity { get; set; }
    public bool IsChildFriendly { get; set; }
    
    public bool IsSelected { get; set; }

    public List<string> Validate()
    {
        List<string> errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Tour name is required.");
        
        if (string.IsNullOrWhiteSpace(From))
            errors.Add("Start location is required.");

        if (string.IsNullOrWhiteSpace(To))
            errors.Add("Destination is required.");

        if (string.IsNullOrWhiteSpace(TransportType))
            errors.Add("Transport type is required.");
        
        return errors;
    }
}
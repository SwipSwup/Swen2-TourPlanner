using System.ComponentModel.DataAnnotations;

public class TourLog
{
    [Required]
    public string Date { get; set; }

    [Required]
    public string Duration { get; set; }

    [Required]
    public string Distance { get; set; }
}
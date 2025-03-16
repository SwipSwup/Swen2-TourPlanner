using System.ComponentModel.DataAnnotations;

public class Tour
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string From { get; set; }

    [Required]
    public string To { get; set; }

    [Required]
    public string TransportType { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Distance must be a positive value.")]
    public double Distance { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Estimated time must be a positive value.")]
    public double EstimatedTime { get; set; }

    public string ImagePath { get; set; }

    public bool IsSelected { get; set; }
}
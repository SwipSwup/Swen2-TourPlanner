using System.ComponentModel.DataAnnotations;

public class TourLog
{
    [Required(ErrorMessage = "Date is required.")]
    public string Date { get; set; }

    [Required(ErrorMessage = "Duration is required.")]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Duration must be a numeric value.")]
    public string Duration { get; set; }

    [Required(ErrorMessage = "Distance is required.")]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Distance must be a numeric value.")]
    public string Distance { get; set; }

    public bool IsSelected { get; set; }
}
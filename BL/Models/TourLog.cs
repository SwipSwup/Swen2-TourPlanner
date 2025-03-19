using System.ComponentModel.DataAnnotations;

namespace BL.Models;

public class TourLog
{
    [Required(ErrorMessage = "Date is required.")]
    [RegularExpression(@"^\d{2}\.\d{2}\.\d{4}$", ErrorMessage = "Date must be in the format dd.mm.yyyy.")]
    public string Date { get; set; }

    [Required(ErrorMessage = "Duration is required.")]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Duration must be a numeric value.")]
    public string Duration { get; set; }

    [Required(ErrorMessage = "Distance is required.")]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Distance must be a numeric value.")]
    public string Distance { get; set; }

    public bool IsSelected { get; set; }

    public List<string> Validate()
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(this);

        bool isValid = Validator.TryValidateObject(this, context, validationResults, true);

        List<string> errors = new List<string>();
        foreach (var validationResult in validationResults)
        {
            errors.Add(validationResult.ErrorMessage);
        }

        return errors;
    }
}
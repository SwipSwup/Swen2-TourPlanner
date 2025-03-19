using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

public class Tour
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Starting location is required.")]
    public string From { get; set; }

    [Required(ErrorMessage = "Destination is required.")]
    public string To { get; set; }

    [Required(ErrorMessage = "Transport Type is required.")]
    public string TransportType { get; set; }

    [Required(ErrorMessage = "Distance is required.")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Distance must be a positive number.")]
    public double Distance { get; set; }

    [Required(ErrorMessage = "Estimated time is required.")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Estimated time must be a positive number.")]
    public double EstimatedTime { get; set; }

    public string ImagePath { get; set; }

    public bool IsSelected { get; set; }

    // TourLogs collection
    public ObservableCollection<TourLog> TourLogs { get; set; } = new ObservableCollection<TourLog>();
    public TourLog SelectedTourLog { get; set; } // To keep track of the selected TourLog for deletion/editing


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

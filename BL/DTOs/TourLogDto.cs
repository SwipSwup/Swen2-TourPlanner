namespace BL.DTOs;

public class TourLogDto
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public float TotalDistance { get; set; }
    public TimeSpan TotalTime { get; set; }
    public int Rating { get; set; }


    public bool IsSelected { get; set; }
    
    public List<string> Validate()
    {
        List<string> errors = new List<string>();

        if (Id <= 0)
            errors.Add("Id must be greater than 0.");

        if (DateTime == default)
            errors.Add("DateTime is required.");

        if (string.IsNullOrWhiteSpace(Comment))
            errors.Add("Comment is required.");

        if (Difficulty < 1 || Difficulty > 5)
            errors.Add("Difficulty must be between 1 and 5.");

        if (TotalDistance <= 0)
            errors.Add("Total distance must be greater than 0.");

        if (TotalTime.TotalMinutes <= 0)
            errors.Add("Total time must be greater than 0.");

        if (Rating < 1 || Rating > 5)
            errors.Add("Rating must be between 1 and 5.");

        return errors;
    }



}
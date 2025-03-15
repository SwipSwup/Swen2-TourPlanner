using System.ComponentModel.DataAnnotations;

public class Tour
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public string ImagePath { get; set; }
}
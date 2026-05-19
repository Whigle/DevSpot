using System.ComponentModel.DataAnnotations;

namespace DevSpot.ViewModels;

public class CompanyViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "Company name can't be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Company description can't be longer than 500 characters")]
    public string? Description { get; set; }

    [StringLength(200, ErrorMessage = "Website URL can't be longer than 200 characters")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? WebsiteUrl { get; set; }
}
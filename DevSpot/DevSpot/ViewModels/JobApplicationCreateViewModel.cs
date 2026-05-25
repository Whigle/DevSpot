using System.ComponentModel.DataAnnotations;

namespace DevSpot.ViewModels;

public class JobApplicationCreateViewModel
{
    public int JobPostingId { get; set; }

    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "CandidateMessage can't be longer than 1000 characters")]
    public string? CandidateMessage { get; set; }

    [Required(ErrorMessage = "CV is required")]
    public IFormFile CvFile { get; set; } = null!;
}
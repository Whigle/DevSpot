using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevSpot.Data;

namespace DevSpot.Models;

public class JobApplication
{
    public int Id { get; set; }
    
    [Required]
    public int JobPostingId { get; set; }
    
    [ForeignKey(nameof(JobPostingId))]
    public JobPosting? JobPosting { get; set; }
    
    [Required]
    public string CandidateId { get; set; } = string.Empty;
    
    [ForeignKey(nameof(CandidateId))]
    public ApplicationUser? Candidate { get; set; }
    
    [Required]
    public string CvFilePath { get; set; } = string.Empty;
    
    public string? CandidateMessage { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public JobApplicationStatus JobApplicationStatus { get; set; } = JobApplicationStatus.Submitted;
}
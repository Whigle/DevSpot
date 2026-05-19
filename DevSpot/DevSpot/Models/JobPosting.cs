using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Models
{
    public class JobPosting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Location can't be longer than 100 characters")]
        public string Location { get; set; } = string.Empty;

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; }

        [Required] 
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))] 
        public IdentityUser User { get; set; }

        public WorkType? WorkType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
        public decimal? Salary { get; set; }

        [StringLength(10, ErrorMessage = "Currency code can't be longer than 10 characters")]
        public string? SalaryCurrency { get; set; } = "PLN";
    }
}
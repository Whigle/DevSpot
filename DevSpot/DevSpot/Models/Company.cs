using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevSpot.Data;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Company name can't be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Company description can't be longer than 500 characters")]
        public string? Description { get; set; }

        [StringLength(200, ErrorMessage = "Website URL can't be longer than 200 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? WebsiteUrl { get; set; }
        
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}

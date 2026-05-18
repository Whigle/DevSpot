using DevSpot.Models;
using System.ComponentModel.DataAnnotations;

namespace DevSpot.ViewModels
{
	public class JobPostingViewModel
	{
		[Required]
		[StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
		public string Title { get; set; } = string.Empty;
		[Required]
		[StringLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
		public string Description { get; set; } = string.Empty;
		[Required]
		[StringLength(100, ErrorMessage = "Company name can't be longer than 100 characters")]
		public string Company { get; set; } = string.Empty;
		[Required]
		[StringLength(100, ErrorMessage = "Location can't be longer than 100 characters")]
		public string Location { get; set; } = string.Empty;
		[Required]
		public WorkType WorkType { get; set; } = WorkType.OnSite;

		[Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
		public decimal? Salary { get; set; }

		[StringLength(10, ErrorMessage = "Currency code can't be longer than 10 characters")]
		public string? SalaryCurrency { get; set; } = "PLN";
	}
}

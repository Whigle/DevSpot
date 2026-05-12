using DevSpot.Models;
using System.ComponentModel.DataAnnotations;

namespace DevSpot.ViewModels
{
	public class JobPostingViewModel
	{
		[Required]
		public string Title { get; set; } = string.Empty;
		[Required]
		public string Description { get; set; } = string.Empty;
		[Required]
		public string Company { get; set; } = string.Empty;
		[Required]
		public string Location { get; set; } = string.Empty;
		[Required]
		public WorkType WorkType { get; set; } = WorkType.OnSite;
	}
}

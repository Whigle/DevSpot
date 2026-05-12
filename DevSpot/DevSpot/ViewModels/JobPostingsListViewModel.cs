using DevSpot.Models;

namespace DevSpot.ViewModels
{
	public class JobPostingsListViewModel
	{
		public IEnumerable<JobPosting> Items { get; set; } = Enumerable.Empty<JobPosting>();
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public JobPostingFilterOptions Filters { get; set; } = new();
	}
}

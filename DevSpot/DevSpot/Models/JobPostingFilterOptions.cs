namespace DevSpot.Models
{
	public class JobPostingFilterOptions
	{
		public string? SearchTitle { get; set; }
		public string? Location { get; set; }
		public WorkType? WorkType { get; set; }
		public string SortBy { get; set; } = "date_desc";
	}
}

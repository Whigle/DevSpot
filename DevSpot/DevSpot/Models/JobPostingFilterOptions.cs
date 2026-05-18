namespace DevSpot.Models
{
	public class JobPostingFilterOptions
	{
		public string? SearchTitle { get; set; }
		public string? Location { get; set; }
		public WorkType? WorkType { get; set; }
		public string SortBy { get; set; } = "date_desc";
	}

	public static class JobPostingSortOptions
	{
		public const string DateDesc = "date_desc";
		public const string DateAsc = "date_asc";
		public const string SalaryDesc = "salary_desc";
		public const string SalaryAsc = "salary_asc";
	}
}

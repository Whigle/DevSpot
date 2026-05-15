namespace DevSpot.ViewModels
{
	public class JobPostingEditViewModel
	{
		public int Id { get; set; }
		public JobPostingViewModel JobPosting { get; set; } = new();
	}
}

using DevSpot.Models;

namespace DevSpot.Repositories
{
	public interface IJobPostingRepository : IRepository<JobPosting>
	{
		Task<IEnumerable<JobPosting>> GetFilteredAsync(JobPostingFilterOptions filters, string? userId = null);
	}
}

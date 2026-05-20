using DevSpot.Models;

namespace DevSpot.Repositories
{
	public interface IJobPostingRepository : IRepository<JobPosting>
	{
		public IQueryable<JobPosting> GetFilteredQuery(JobPostingFilterOptions filters, string? userId = null);
	}
}

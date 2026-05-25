using DevSpot.Models;
using DevSpot.ViewModels.FilterOptions;

namespace DevSpot.Repositories.Interfaces
{
	public interface IJobPostingRepository : IRepository<JobPosting>
	{
		public IQueryable<JobPosting> GetFilteredQuery(JobPostingFilterOptions filters, string? userId = null);
		public Task<JobPosting?> GetByIdWithCompanyAsync(int id);
	}
}

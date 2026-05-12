using DevSpot.Data;
using DevSpot.Models;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Repositories
{
	public class JobPostingRepository : IJobPostingRepository
	{
		private readonly ApplicationDbContext _context;

		public JobPostingRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(JobPosting entity)
		{
			await _context.JobPostings.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var jobPosting = await _context.JobPostings.FindAsync(id);

			if (jobPosting == null)
			{
				throw new KeyNotFoundException();
			}

			_context.JobPostings.Remove(jobPosting);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<JobPosting>> GetAllAsync()
		{
			return await _context.JobPostings.ToListAsync();
		}

		public async Task<JobPosting> GetByIdAsync(int id)
		{
			var jobPosting = await _context.JobPostings.FindAsync(id);

			if (jobPosting == null)
			{
				throw new KeyNotFoundException();
			}

			return jobPosting;
		}

		public async Task<IEnumerable<JobPosting>> GetFilteredAsync(JobPostingFilterOptions filters, string? userId = null)
		{
			var query = _context.JobPostings.AsQueryable();

			if(userId != null) {
				query = query.Where(posting => posting.UserId == userId);
			}

			if(!string.IsNullOrEmpty(filters.SearchTitle)) {
				query = query.Where(posting => posting.Title.Contains(filters.SearchTitle));
			}

			if(filters.WorkType != null) {
				query = query.Where(posting => posting.WorkType.Equals(filters.WorkType));
			}

			if(filters.Location != null)
			{
				query = query.Where(posting => posting.Location.Contains(filters.Location));
			}

			query = filters.SortBy switch
			{
				"date_asc" => query.OrderBy(posting => posting.PostedDate),
				_ => query.OrderByDescending(posting => posting.PostedDate)
			};

			return await query.ToListAsync();
		}

		public async Task UpdateAsync(JobPosting entity)
		{
			_context.JobPostings.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}

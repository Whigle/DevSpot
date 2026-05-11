using DevSpot.Data;
using DevSpot.Models;
using DevSpot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Tests
{
	public class JobPostingRepositoryTests
	{
		private readonly DbContextOptions<ApplicationDbContext> _options;

		public JobPostingRepositoryTests()
		{
			_options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("JobPostingDb")
				.Options;
		}

		private ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);

		[Fact]
		public async Task AddAsync_ShouldAddJobPosting()
		{
			var db = CreateDbContext();
			var repository = new JobPostingRepository(db);
			var jobPosting = new JobPosting()
			{
				Title = "Test Title",
				Description = "Test Description",
				PostedDate = DateTime.Now,
				Company = "Test Company",
				Location = "Test Location",
				UserId = "TestUserId"
			};

			await repository.AddAsync(jobPosting);

			var result = await db.JobPostings.FindAsync(jobPosting.Id);

			Assert.NotNull(result);
			Assert.Equal("Test Description", result.Description);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnJobPosting()
		{
			var db = CreateDbContext();
			var repository = new JobPostingRepository(db);
			var jobPosting = new JobPosting()
			{
				Title = "Test Title",
				Description = "Test Description",
				PostedDate = DateTime.Now,
				Company = "Test Company",
				Location = "Test Location",
				UserId = "TestUserId"
			};

			await db.JobPostings.AddAsync(jobPosting);
			await db.SaveChangesAsync();

			var result = await repository.GetByIdAsync(jobPosting.Id);

			Assert.NotNull(result);
			Assert.Equal("Test Title", result.Title);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
		{
			var db = CreateDbContext();
			var repository = new JobPostingRepository(db);

			await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(999));
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllJobPostings()
		{
			var db = CreateDbContext();
			var repository = new JobPostingRepository(db);

			var jobPosting1 = new JobPosting()
			{
				Title = "Test Title1",
				Description = "Test Description1",
				PostedDate = DateTime.Now,
				Company = "Test Company1",
				Location = "Test Location1",
				UserId = "TestUserId1"
			};

			var jobPosting2 = new JobPosting()
			{
				Title = "Test Title2",
				Description = "Test Description2",
				PostedDate = DateTime.Now,
				Company = "Test Company2",
				Location = "Test Location2",
				UserId = "TestUserId2"
			};

			await db.JobPostings.AddRangeAsync(jobPosting1, jobPosting2);
			await db.SaveChangesAsync();

			var result = await repository.GetAllAsync();

			Assert.NotNull(result);
			Assert.True(result.Count() > 2);
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateJobPosting()
		{
			var db = CreateDbContext();
			var repository = new JobPostingRepository(db);

			var jobPosting = new JobPosting()
			{
				Title = "Test Title",
				Description = "Test Description",
				PostedDate = DateTime.Now,
				Company = "Test Company",
				Location = "Test Location",
				UserId = "TestUserId"
			};

			await db.JobPostings.AddAsync(jobPosting);
			await db.SaveChangesAsync();

			jobPosting.Description = "Updated Description";

			await repository.UpdateAsync(jobPosting);

			var result = await db.JobPostings.FindAsync(jobPosting.Id);

			Assert.NotNull(result);
			Assert.Equal("Updated Description", result.Description);
		}

		[Fact]
		public async Task DeleteAsync_ShouldDeledeJobPosting() 
		{
			var db = CreateDbContext();
			var repository = new JobPostingRepository(db);

			var jobPosting = new JobPosting()
			{
				Title = "Test Title",
				Description = "Test Description",
				PostedDate = DateTime.Now,
				Company = "Test Company",
				Location = "Test Location",
				UserId = "TestUserId"
			};

			await db.JobPostings.AddAsync(jobPosting);
			await db.SaveChangesAsync();

			await repository.DeleteAsync(jobPosting.Id);

			var result = db.JobPostings.Find(jobPosting.Id);

			Assert.Null(result);
		}
	}
}

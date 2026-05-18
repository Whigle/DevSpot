using DevSpot.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<JobPosting> JobPostings { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<JobPosting>()
				.Property(p => p.Salary)
				.HasPrecision(18, 2);
		}
	}
}

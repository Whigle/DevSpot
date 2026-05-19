using DevSpot.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<JobPosting> JobPostings { get; set; }
		public DbSet<Company> Companies { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<JobPosting>()
				.Property(p => p.Salary)
				.HasPrecision(18, 2);

			// Configure Company - JobPosting relationship (one-to-many)
			modelBuilder.Entity<Company>()
				.HasMany(c => c.JobPostings)
				.WithOne(j => j.Company)
				.HasForeignKey(j => j.CompanyId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}

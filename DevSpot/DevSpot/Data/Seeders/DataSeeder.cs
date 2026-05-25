using Bogus;
using DevSpot.Models;
using DevSpot.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Data.Seeders
{
    public static class DataSeeder
    {
        private const int JOB_OFFERS_PER_EMPLOYEE = 50;
        
        public static async Task SeedBusinessDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (await context.JobPostings.AnyAsync()) return;

            var employers = await userManager.GetUsersInRoleAsync("Employer");
            if (!employers.Any()) return;
            
            var existingCompanyUserIds = await context.Companies
                .AsNoTracking()
                .Select(c => c.UserId)
                .ToListAsync();

            PrepareFakeDataRegs(out var companyFaker, out var jobPostingFaker);

            foreach (var employer in employers)
            {
                var hasCompany = existingCompanyUserIds.Contains(employer.Id);
                if (hasCompany) continue;

                var newCompany = AddCompany(context, companyFaker, employer);
                AddPostings(context, jobPostingFaker, newCompany, employer);
            }
                
            await context.SaveChangesAsync();

            return;

            static void PrepareFakeDataRegs(out Faker<Company> companyFaker, out Faker<JobPosting> jobPostingFaker)
            {
                companyFaker = new Faker<Company>("pl")
                    .RuleFor(c => c.Name, f => f.Company.CompanyName())
                    .RuleFor(c => c.Description, f => f.Company.CatchPhrase())
                    .RuleFor(c => c.WebsiteUrl, f => f.Internet.Url());

                jobPostingFaker = new Faker<JobPosting>("pl")
                    .RuleFor(j => j.Title, f => f.PickRandom(new[] { "Junior .NET Developer", "C# Developer", "Backend Engineer", "Fullstack Developer" }))
                    .RuleFor(j => j.Description, f => f.Lorem.Paragraphs(2))
                    .RuleFor(j => j.Location, f => f.PickRandom(new[] { "Warszawa", "Kraków", "Wrocław", "Gdańsk", "Zdalnie" }))
                    .RuleFor(j => j.WorkType, f => f.PickRandom<WorkType>())
                    .RuleFor(j => j.Salary, f => f.Random.Decimal(5000, 18000))
                    .RuleFor(j => j.SalaryCurrency, "PLN")
                    .RuleFor(j => j.PostedDate, f => f.Date.Recent(30))
                    .RuleFor(j => j.IsApproved, true);
            }

            static Company AddCompany(ApplicationDbContext context, Faker<Company> companyFaker, ApplicationUser employer)
            {
                var newCompany = companyFaker.Generate();
                newCompany.UserId = employer.Id;
                context.Companies.Add(newCompany);
                return newCompany;
            }

            static void AddPostings(ApplicationDbContext context, Faker<JobPosting> jobPostingFaker, Company newCompany, ApplicationUser employer)
            {
                var postings = jobPostingFaker.Generate(JOB_OFFERS_PER_EMPLOYEE);
                foreach (var posting in postings)
                {
                    posting.Company = newCompany;
                    posting.UserId = employer.Id;
                }

                context.JobPostings.AddRange(postings);
            }
        }
    }
}
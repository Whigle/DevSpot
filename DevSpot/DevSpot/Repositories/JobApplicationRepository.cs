using DevSpot.Data;
using DevSpot.Models;
using DevSpot.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Repositories;

public class JobApplicationRepository(ApplicationDbContext context) : IJobApplicationRepository
{
    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await context.JobApplications.ToListAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(int id)
    {
        return await context.JobApplications.FindAsync(id);
    }

    public async Task AddAsync(JobApplication entity)
    {
        await context.JobApplications.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobApplication entity)
    {
        context.JobApplications.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var jobApplication = await context.JobApplications.FindAsync(id);

        if (jobApplication == null)
        {
            throw new KeyNotFoundException();
        }

        context.JobApplications.Remove(jobApplication);
        await context.SaveChangesAsync();
    }

    public async Task<List<int>> GetAppliedJobIdsAsync(string userId)
    {
        var appliedJobIds = await context.JobApplications
            .Where(a => a.CandidateId == userId)
            .Select(a => a.JobPostingId)
            .ToListAsync();

        return appliedJobIds;
    }

    public async Task<bool> IsAppliedAsync(int jobPostingId, string userId)
    {
        var alreadyApplied = await context.JobApplications
            .AnyAsync(a => a.JobPostingId == jobPostingId && a.CandidateId == userId);

        return alreadyApplied;
    }
}
using DevSpot.Models;

namespace DevSpot.Repositories.Interfaces;

public interface IJobApplicationRepository : IRepository<JobApplication>
{
    public Task<List<int>> GetAppliedJobIdsAsync(string userId);
    public Task<bool> IsAppliedAsync(int jobPostingId, string userId);
}
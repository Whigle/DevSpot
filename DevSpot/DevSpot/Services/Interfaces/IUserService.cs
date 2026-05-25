using DevSpot.Models;

namespace DevSpot.Services.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserWithCompanyAsync(string userId);
}
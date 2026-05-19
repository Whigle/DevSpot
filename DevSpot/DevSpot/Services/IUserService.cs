using DevSpot.Data;

namespace DevSpot.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetUserWithCompanyAsync(string userId);
}
using DevSpot.Constants;
using DevSpot.Models;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Data.Seeders
{
	public class UserSeeder
	{
		public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			await CreateUserWithRole(userManager, "admin@devsopt.com", "Admin123!", Roles.ADMIN);
			await CreateUserWithRole(userManager, "jobseeker@devsopt.com", "JobSeeker123!", Roles.JOB_SEEKER);
			await CreateUserWithRole(userManager, "employer@devsopt.com", "Employer123!", Roles.EMPLOYER);
		}

		private static async Task CreateUserWithRole(UserManager<ApplicationUser> userManager, string email, string password, string role)
		{
			if (await userManager.FindByEmailAsync(email) == null)
			{
				var user = new ApplicationUser
				{
					Email = email,
					EmailConfirmed = true,
					UserName = email
				};

				var result = await userManager.CreateAsync(user, password);

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, role);
				}
				else
				{
					throw new Exception($"Failed creating user with email {user.Email}. Errors: {string.Join(", ", result.Errors)}");
				}
			}
		}
	}
}

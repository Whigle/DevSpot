using DevSpot.Data;
using DevSpot.Models;
using DevSpot.Repositories;
using DevSpot.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevSpot
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
			});

			builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
			})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			//builder.Services.AddTransient
			//builder.Services.AddSingleton
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IJobPostingRepository, JobPostingRepository>();
			builder.Services.AddScoped<IRepository<JobPosting>>(provider =>
				provider.GetRequiredService<IJobPostingRepository>());
			builder.Services.AddScoped<IRepository<Company>, CompanyRepository>();

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error/500");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			
			app.UseStatusCodePagesWithReExecute("/Error/{0}");

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				RoleSeeder.SeedRolesAsync(services).Wait();
				UserSeeder.SeedUsersAsync(services).Wait();
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			app.UseAuthorization();

			app.MapRazorPages();

			app.MapStaticAssets();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=JobPostings}/{action=Index}/{id?}")
				.WithStaticAssets();

			app.Run();
		}
	}
}

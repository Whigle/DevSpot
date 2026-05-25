using DevSpot.Constants;
using DevSpot.Models;
using DevSpot.Models.Enums;
using DevSpot.Repositories.Interfaces;
using DevSpot.Services.Interfaces;
using DevSpot.ViewModels;
using DevSpot.ViewModels.FilterOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Controllers
{
	[Authorize]
	public class JobPostingsController(
		IJobPostingRepository jobPostingRepository,
		UserManager<ApplicationUser> userManager,
		IRepository<Company> companiesRepository,
		IUserService userService,
		IFileService fileService,
		IJobApplicationRepository applicationRepository)
		: Controller
	{
		
		[AllowAnonymous]
		public async Task<IActionResult> Index(
			int page = 1,
			string? searchTitle = null,
			string? location = null,
			WorkType? workType = null,
			string sortBy = "date_desc")
		{
			const int pageSize = 10;

			var filters = new JobPostingFilterOptions()
			{
				SearchTitle = searchTitle,
				Location = location,
				WorkType = workType,
				SortBy = sortBy
			};

			var userId = userManager.GetUserId(User);

			var query = jobPostingRepository.GetFilteredQuery(filters, User.IsInRole(Roles.EMPLOYER) ? userId : null);

			var totalCount = await query.CountAsync();
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			EnsureValidPageParameters();

			var items = await query
				.Include(j => j.Company)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var appliedJobIds = new List<int>();
			
			if (userId != null && User.IsInRole(Roles.JOB_SEEKER))
			{
				appliedJobIds = await applicationRepository.GetAppliedJobIdsAsync(userId);
			}

			var vm = new JobPostingsListViewModel
			{
				Items = items,
				TotalPages = totalPages,
				CurrentPage = page,
				Filters = filters,
				AppliedJobIds = appliedJobIds.ToHashSet()
			};

			return View(vm);
			
			void EnsureValidPageParameters()
			{
				if (page < 1) page = 1;
				if (totalPages == 0) totalPages = 1;
				if (page > totalPages) page = totalPages;
			}
		}

		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		public async Task<IActionResult> Create()
		{
			var userId = userManager.GetUserId(User)!;

			var currentUser = await userService.GetUserWithCompanyAsync(userId);

			if (currentUser!.Company == null)
			{
				var targetUrl = Url.Action("Create", "JobPostings");
				return RedirectToAction(nameof(CompaniesController.Create), 
					"Companies", 
					new { returnUrl = targetUrl });
			}
			
			return View();
		}
		
		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(JobPostingViewModel jobPostingVm)
		{
			if (!ModelState.IsValid)
			{
				return View(jobPostingVm);
			}
			
			var userId = userManager.GetUserId(User)!;	//null forgiving, UserId can not be null as we are in Authorized method
			
			var currentUser = await userService.GetUserWithCompanyAsync(userId);

			if (currentUser!.Company == null)
			{
				return RedirectToAction(
					nameof(CompaniesController.Create), 
					"Companies", 
					new { returnUrl = "/JobPostings/Create" });
			}
			
			var jobPosting = new JobPosting
			{
				Title = jobPostingVm.Title,
				Description = jobPostingVm.Description,
				CompanyId = currentUser.Company.Id,
				Location = jobPostingVm.Location,
				UserId = userId!, 
				WorkType = jobPostingVm.WorkType,
				Salary = jobPostingVm.Salary,
				SalaryCurrency = jobPostingVm.SalaryCurrency
			};

			await jobPostingRepository.AddAsync(jobPosting);

			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		public async Task<IActionResult> Edit(int id)
		{
			var userId = userManager.GetUserId(User)!;
			var jobPosting = await jobPostingRepository.GetByIdAsync(id);

			if (jobPosting == null)
			{
				return NotFound();
			}

			if (jobPosting.UserId != userId)
			{
				return Forbid();
			}

			var jobPostingEdit = new JobPostingEditViewModel();

			jobPostingEdit.Id = id;

			jobPostingEdit.JobPosting = new JobPostingViewModel()
			{
				Title = jobPosting.Title,
				CompanyId = jobPosting.CompanyId,
				Description = jobPosting.Description,
				Location = jobPosting.Location,
				WorkType = jobPosting.WorkType ?? WorkType.Remote,
				Salary = jobPosting.Salary,
				SalaryCurrency = jobPosting.SalaryCurrency
			};

			return View(jobPostingEdit);
		}

		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(JobPostingEditViewModel jobPostingEditVm)
		{
			if (!ModelState.IsValid)
			{
				return View(jobPostingEditVm);
			}

			var userId = userManager.GetUserId(User)!;
			var jobPosting = await jobPostingRepository.GetByIdAsync(jobPostingEditVm.Id);

			if (jobPosting == null)
			{
				return NotFound();
			}

			if (!User.IsInRole(Roles.ADMIN) && jobPosting.UserId != userId)
			{
				return Forbid();
			}

			try
			{
				jobPosting.Title = jobPostingEditVm.JobPosting.Title;
				jobPosting.Description = jobPostingEditVm.JobPosting.Description;
				jobPosting.CompanyId = jobPostingEditVm.JobPosting.CompanyId;
				jobPosting.Location = jobPostingEditVm.JobPosting.Location;
				jobPosting.WorkType = jobPostingEditVm.JobPosting.WorkType;
				jobPosting.Salary = jobPostingEditVm.JobPosting.Salary;
				jobPosting.SalaryCurrency = jobPostingEditVm.JobPosting.SalaryCurrency;

				await jobPostingRepository.UpdateAsync(jobPosting);

				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				ModelState.AddModelError(string.Empty, "Record has been modified by another user.");
				return View(jobPostingEditVm);
			}
		}

		[Authorize(Roles = $"{Roles.JOB_SEEKER}")]
		public async Task<IActionResult> Apply(int id)
		{
			var jobPosting = await jobPostingRepository.GetByIdWithCompanyAsync(id);

			if (jobPosting == null)
			{
				return NotFound();
			}
			
			var userId = userManager.GetUserId(User);


			var alreadyApplied = await applicationRepository.IsAppliedAsync(id, userId!);
			
			if(alreadyApplied)
			{
				TempData["ErrorMessage"] = "User already applied for this offer.";
				return RedirectToAction(nameof(Index));
			}

			var vm = new JobApplicationCreateViewModel()
			{
				JobPostingId = jobPosting.Id,
				JobTitle = jobPosting.Title,
				CompanyName = jobPosting.Company?.Name ?? "Not defined"
			};
			
			return View(vm);
		}

		[HttpPost]
		[Authorize(Roles = $"{Roles.JOB_SEEKER}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Apply(JobApplicationCreateViewModel vm)
		{
			var jobPosting = await jobPostingRepository.GetByIdAsync(vm.JobPostingId);

			if (jobPosting == null)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			try
			{
				var savedFilePath = await fileService.UploadCvAsync(vm.CvFile);

				var application = new JobApplication()
				{
					JobPostingId = vm.JobPostingId,
					CandidateId = userManager.GetUserId(User)!,
					CandidateMessage = vm.CandidateMessage,
					CvFilePath = savedFilePath,
					AppliedAt = DateTime.UtcNow,
					JobApplicationStatus = JobApplicationStatus.Submitted
				};

				await applicationRepository.AddAsync(application);

				TempData["SuccessMessage"] = "Application has been sent.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				return View(vm);
			}
		}

		[HttpDelete]
		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			var jobPosting = await jobPostingRepository.GetByIdAsync(id);

			if (jobPosting == null)
			{
				return NotFound();
			}

			var userId = userManager.GetUserId(User);

			if (User.IsInRole(Roles.ADMIN) == false && jobPosting.UserId != userId)
			{
				return Forbid();
			}

			await jobPostingRepository.DeleteAsync(id);

			return Ok();
		}
	}
}

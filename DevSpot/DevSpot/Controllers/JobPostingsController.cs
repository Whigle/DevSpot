using DevSpot.Constants;
using DevSpot.Models;
using DevSpot.Repositories;
using DevSpot.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSpot.Controllers
{
	[Authorize]
	public class JobPostingsController : Controller
	{
		private readonly IJobPostingRepository _repository;
		private readonly UserManager<IdentityUser> _userManager;

		public JobPostingsController(IJobPostingRepository repository, UserManager<IdentityUser> userManager)
		{
			_repository = repository;
			_userManager = userManager;
		}

		[AllowAnonymous]
		public async Task<IActionResult> Index(
			int page = 1,
			string? searchTitle = null,
			string? location = null,
			WorkType? workType = null,
			string sortBy = "date_desc")
		{
			const int pageSize = 2;

			var filters = new JobPostingFilterOptions()
			{
				SearchTitle = searchTitle,
				Location = location,
				WorkType = workType,
				SortBy = sortBy
			};

			string? userId = User.IsInRole(Roles.EMPLOYER) ? _userManager.GetUserId(User) : null;

			var jobPostings = await _repository.GetFilteredAsync(filters, userId);

			var totalCount = jobPostings.Count();
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			EnsureValidPageParameters(ref page, ref totalPages);

			var items = jobPostings
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var vm = new JobPostingsListViewModel
			{
				Items = items,
				TotalPages = totalPages,
				CurrentPage = page,
				Filters = filters,
			};

			return View(vm);
		}

		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		public IActionResult Create()
		{
			return View();
		}

		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		public async Task<IActionResult> Edit(int id)
		{
			var userId = _userManager.GetUserId(User)!;
			var jobPosting = await _repository.GetByIdAsync(id);

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
				Company = jobPosting.Company,
				Description = jobPosting.Description,
				Location = jobPosting.Location,
				WorkType = jobPosting.WorkType ?? WorkType.Remote
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

			var userId = _userManager.GetUserId(User)!;
			var jobPosting = await _repository.GetByIdAsync(jobPostingEditVm.Id);

			if (jobPosting == null)
			{
				return NotFound();
			}

			if (jobPosting.UserId != userId)
			{
				return Forbid();
			}

			jobPosting.Title = jobPostingEditVm.JobPosting.Title;
			jobPosting.Description = jobPostingEditVm.JobPosting.Description;
			jobPosting.Company = jobPostingEditVm.JobPosting.Company;
			jobPosting.Location = jobPostingEditVm.JobPosting.Location;
			jobPosting.WorkType = jobPostingEditVm.JobPosting.WorkType;

			await _repository.UpdateAsync(jobPosting);

			return RedirectToAction(nameof(Index));
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

			var jobPosting = new JobPosting
			{
				Title = jobPostingVm.Title,
				Description = jobPostingVm.Description,
				Company = jobPostingVm.Company,
				Location = jobPostingVm.Location,
				UserId = _userManager.GetUserId(User)!, //null forgiving, UserId can not be null as we are in Authorized method
				WorkType = jobPostingVm.WorkType,
			};

			await _repository.AddAsync(jobPosting);

			return RedirectToAction(nameof(Index));
		}

		[HttpDelete]
		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			var jobPosting = await _repository.GetByIdAsync(id);

			if (jobPosting == null)
			{
				return NotFound();
			}

			var userId = _userManager.GetUserId(User);

			if (User.IsInRole(Roles.ADMIN) == false && jobPosting.UserId != userId)
			{
				return Forbid();
			}

			await _repository.DeleteAsync(id);

			return Ok();
		}

		private void EnsureValidPageParameters(ref int page, ref int totalPages)
		{
			if (page < 1) page = 1;
			if (totalPages == 0) totalPages = 1;
			if (page > totalPages) page = totalPages;
		}
	}
}

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
		[HttpPost]
		public async Task<IActionResult> Create(JobPostingViewModel jobPostingVm)
		{
			if (ModelState.IsValid)
			{
				var jobPosting = new JobPosting
				{
					Title = jobPostingVm.Title,
					Description = jobPostingVm.Description,
					Company = jobPostingVm.Company,
					Location = jobPostingVm.Location,
					UserId = _userManager.GetUserId(User),
					WorkType = jobPostingVm.WorkType,
				};

				await _repository.AddAsync(jobPosting);

				return RedirectToAction(nameof(Index));
			}

			return View(jobPostingVm);
		}

		[HttpDelete]
		[Authorize(Roles = $"{Roles.ADMIN}, {Roles.EMPLOYER}")]
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

using DevSpot.Constants;
using DevSpot.Data;
using DevSpot.Models;
using DevSpot.Repositories;
using DevSpot.Services;
using DevSpot.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSpot.Controllers;

public class CompaniesController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Company> _companiesRepository;
    private readonly IUserService _userService;

    public CompaniesController(UserManager<ApplicationUser> userManager,
        IRepository<Company> companiesRepository,
        IUserService userService)
    {
        _userManager = userManager;
        _companiesRepository = companiesRepository;
        _userService = userService;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = $"{Roles.EMPLOYER}")]
    public IActionResult Create(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{Roles.EMPLOYER}")]
    public async Task<IActionResult> Create(CompanyViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;
        var currentUser = await _userService.GetUserWithCompanyAsync(userId);

        if (currentUser!.Company != null)  // ZMIEN NA !=
        {
            ModelState.AddModelError("", "You already have a company profile.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var company = new Company
        {
            Name = model.Name,
            Description = model.Description,
            WebsiteUrl = model.WebsiteUrl,
            UserId = userId, //null forgiving, UserId can not be null as we are in Authorized method
        };

        await _companiesRepository.AddAsync(company);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index), "JobPostings");
    }
}
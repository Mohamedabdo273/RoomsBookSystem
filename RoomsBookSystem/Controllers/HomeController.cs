using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using infrastructure.Services.IService;
using Microsoft.AspNetCore.Authorization;
using RoomsBookSystem.Models.ViewModels;

namespace RoomsBookSystem.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHotelBranchService _hotelBranchService;

    public HomeController(ILogger<HomeController> logger, IHotelBranchService hotelBranchService)
    {
        _logger = logger;
        _hotelBranchService = hotelBranchService;
    }

    public async Task<IActionResult> Index(string searchString, int? page)
    {
        if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
        {
            return RedirectToAction("HotelBranches", "Admin");
        }

        // Default values
        int pageSize = 6;
        int pageNumber = page ?? 1;
        searchString = searchString ?? string.Empty;

        var allHotels = await _hotelBranchService.GetAllAsync();

        if (!string.IsNullOrEmpty(searchString))
        {
            allHotels = allHotels.Where(h =>
                h.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                h.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        int totalItems = allHotels.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

        var hotelBranches = allHotels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new HotelBranchListViewModel
        {
            HotelBranches = hotelBranches,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            PageSize = pageSize,
            SearchString = searchString,
            TotalItems = totalItems,
        };

        return View(viewModel);
    }

    public async Task<IActionResult> BranchDetails(int id)
    {
        var branch = await _hotelBranchService.GetByIdAsync(id);
        if (branch == null)
        {
            return NotFound();
        }
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("HotelBranchDetails", "Admin", new { id = id });
        }

        return View(branch);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

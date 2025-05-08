using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using infrastructure.Services.IService;
using Microsoft.AspNetCore.Authorization;

namespace RoomsBookSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHotelBranchService _hotelBranchService;

    public HomeController(ILogger<HomeController> logger, IHotelBranchService hotelBranchService)
    {
        _logger = logger;
        _hotelBranchService = hotelBranchService;
    }

    public async Task<IActionResult> Index()
    {
        var hotelBranches = await _hotelBranchService.GetAllAsync();

        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("HotelBranches", "Admin");
        }

        return View("Index", hotelBranches);
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

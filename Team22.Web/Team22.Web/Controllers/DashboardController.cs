using Microsoft.AspNetCore.Mvc;
using Team22.Web.Services;

namespace Team22.Web.Controllers;

// Controller for the main dashboard after signing into the application

public class DashboardController : Controller
{

    private readonly DashboardService _dashboard;

    public DashboardController(DashboardService dashboard)
    {
        _dashboard = dashboard;
    }
    
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SponsorIndex()
    {
        return View();
    }

    public IActionResult AdminIndex()
    {
        return View();
    }

    
}
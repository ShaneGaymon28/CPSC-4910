using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Services;

namespace Team22.Web.Controllers;

public class PointController : Controller
{
    private readonly PointService _pointService;
    private readonly UserManager<AppUser> _userManager;
    private readonly UserService _userService;
    
    public PointController(PointService pointService, UserManager<AppUser> userManager, UserService userService)
    {
        _pointService = pointService;
        _userManager = userManager;
        _userService = userService;
    }
    
    // GET
    [HttpGet]
    public async Task<IActionResult> DriverPointHistory()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var bridge = await _userService.GetSponsorUserBridge(user.Id);
        if (bridge.Status == QueryStatus.NotFound)
        {
            return View();
        }
        
        ViewBag.CurrentPoints = bridge.Value.Points != null ? bridge.Value.Points : 0;
        // get point history from DB
        return await _pointService.GetDriverPointHistory(user.Id) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }

    [HttpGet]
    public async Task<IActionResult> SponsorPoint()
    {
        // get sponsor user making request
        var sponsorUser = await _userManager.GetUserAsync(HttpContext.User);
        
        // need to get all drivers associated with sponsor / sponsor organization
        // this is what is is now
        var bridge = await _userService.GetSponsorUserBridgeBySponsorId(1); // TODO: remove hardcoding

        // this is what it should be once I merge the updated AppUser 
        //var bridge = await _userService.GetSponsorUserBridgeBySponsorId(sponsorUser.SponsorId);
        
        return View(bridge.Value);
    }

    [HttpPost]
    public async Task<IActionResult> AddPointChange(int pointChange, string driverId, int currentPoints, string reason)
    {
        var sponsorUser = await _userManager.GetUserAsync(HttpContext.User);

        await _pointService.AddNewPointChange(pointChange, driverId, sponsorUser.Id, currentPoints, reason);

        return RedirectToAction("SponsorPoint");
    }
    
}
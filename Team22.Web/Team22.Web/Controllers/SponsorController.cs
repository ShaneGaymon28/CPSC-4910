using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Team22.Web.Contexts;
using Team22.Web.Services;
using Team22.Web.Enums;
using Team22.Web.Models;

namespace Team22.Web.Controllers;

public class SponsorController: Controller
{
    private readonly SponsorService _sponsorService;
    private readonly Team22Context _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationService _applicationService;

    public SponsorController(SponsorService sponsorService, Team22Context context, UserManager<AppUser> userManager, ApplicationService applicationService)
    {
        _sponsorService = sponsorService;
        _context = context;
        _userManager = userManager;
        _applicationService = applicationService;
    }
    

    public async Task<IActionResult> Company()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SetCompany(string company)
    {
        var httpUser = await _userManager.GetUserAsync(HttpContext.User);
        var dbUser = await _context.AppUser.FirstOrDefaultAsync(s => s.UserName == httpUser.UserName);
        var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.Name == company);
        if (dbUser is null || sponsor is null)
        {
            return View("Company");
        }
        dbUser.SponsorId = sponsor.Id;
        await _context.SaveChangesAsync();
        return RedirectToAction("SponsorAccount", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> DropDriver(int appId, string comment)
    {
        var httpUser = await _userManager.GetUserAsync(HttpContext.User);
        var result = await _applicationService.UpdateApplication(new ApplicationService.UpdateApplicationQuery
        {
            AppId = appId,
            DeciderId = httpUser.Id,
            Accepted = false,
            Reason = comment
        });
        
        if (httpUser.UserRole == UserRole.Admin)
        {
            return RedirectToAction("AdminApplication", "Applications");
        }

        return RedirectToAction("SponsorApplication", "Applications", new { userName = httpUser.UserName });
    }

    [HttpPost]
    public async Task<IActionResult> DropSponsor(int appId, string comment)
    {
        var httpUser = await _userManager.GetUserAsync(HttpContext.User);
        var result = await _applicationService.DropApplication(new ApplicationService.UpdateApplicationQuery
        {
            AppId = appId,
            DeciderId = httpUser.Id,
            Accepted = false,
            Reason = comment
        });

        return RedirectToAction("DriverApplication", "Applications", new { id = httpUser.Id });
    }
    
}
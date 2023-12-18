using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Team22.Web.Services;
using Team22.Web.Enums;
using Team22.Web.Models;

namespace Team22.Web.Controllers;

public class UserController: Controller
{
    private readonly UserService _userService;
    private readonly SponsorService _sponsorService;
    private readonly UserManager<AppUser> _userManager;

    public UserController(UserService userService, SponsorService sponsorService, UserManager<AppUser> userManager)
    {
        _userService = userService;
        _sponsorService = sponsorService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return await _userService.GetAllUsers() switch
        {
            { Status: QueryStatus.Conflict } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };

        return View();
    }
    

    public async Task<IActionResult> AdminCreateAccount()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ViewUser(string userName)
    {
        return await _userService.GetUser(new UserService.GetUserQuery { UserName = userName }) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }

    [HttpPost]
    public async Task<IActionResult> Update(AppUser user)
    {
        // update in user
        await _userService.Update(new UserService.UpdateUserQuery
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserRole = user.UserRole,
            UserName = user.UserName
        });

        return View("ViewUser", user);
    }
    
}
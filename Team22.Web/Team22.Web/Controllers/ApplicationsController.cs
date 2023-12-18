using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Services;
using Team22.Web.ViewModels;

namespace Team22.Web.Controllers;

public class ApplicationsController : Controller
{
    private readonly SponsorService _sponsorService;
    private readonly ApplicationService _applicationService;
    private readonly UserService _userService;
    private readonly UserManager<AppUser> _userManager;

    public ApplicationsController(ApplicationService applicationService, SponsorService sponsorService, UserService userService, UserManager<AppUser> userManager)
    {
        _applicationService = applicationService;
        _sponsorService = sponsorService;
        _userService = userService;
        _userManager = userManager;
    }

    /*
     * GET ALL APPLICATIONS
     * returns all applications to Applications/Index page
     *
     * NOTE: Applications/Index will probably not get used or we can change it
     */
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return await _applicationService.GetAllApplications() switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }
    
    /*
     * GET DRIVER APPLICATIONS
     * returns all applications for a driver to Applications/DriverApplication
     * Params
     *      int id - user id of the driver to get applications for 
     * 
     */
    [HttpGet]
    public async Task<IActionResult> DriverApplication(string id)
    {
        // View available sponsors
        return await _applicationService.GetApplicationsByDriverId(id) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }

    /*
     * GET SPONSOR APPLICATIONS
     * returns all applications for a sponsor to Applications/SponsorApplication
     * Params
     *      int sponsorId - sponsor id of the sponsor user to get applications for
     *
     */
    [HttpGet]
    public async Task<IActionResult> SponsorApplication(string userName)
    {
        var sponsorUser = await _userService.GetUser(new UserService.GetUserQuery
        { UserName = userName });

        if (sponsorUser.Value.SponsorId is null) { return View(); }
        return await _applicationService.GetApplicationsBySponsorId((int)sponsorUser.Value.SponsorId) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }
    

    /*
     * GET ADMIN APPLICATIONS
     * returns all applications to Applications/AdminApplication
     *  
     */
    [HttpGet]
    public async Task<IActionResult> AdminApplication()
    {
        return await _applicationService.GetAllApplications() switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }

    /*
     * APPLY TO A SPONSOR
     * returns the sponsor and driver information for a new application
     * Params
     *      int sponsorId - sponsor id the driver is applying to
     *      int userId - user id of the driver completing the application
     *
     * NOTE: these id values are hardcoded; there is also probably a better way to do this
     */
    [HttpGet]
    public async Task<IActionResult> Apply(int sponsorId)
    {
        // to display sponsor name 
        var sponsor = await _sponsorService.GetSponsor(new SponsorService.GetSponsorQuery
        {
            SponsorId = sponsorId
        });

        ViewData["Sponsor"] = sponsor.Value;
        
        var driver = await _userService.GetUser(new UserService.GetUserQuery
        {
            UserName = "" // todo: update
        });

        ViewData["User"] = driver.Value;
        
        return View();
    }

    /*
     * GET APPLICATION BY ID
     * returns an application for given id
     * Params
     *      int id - id of the application to return
     * 
     */
    [HttpGet]
    public async Task<IActionResult> ViewApplication(int id)
    {
        return await _applicationService.GetApplication(id) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }
    

    /*
     * CREATE A NEW APPLICATION
     * creates a new application
     * Params
     *      Application newApplication - new application to be created
     */
    [HttpPost]
    public async Task<IActionResult> Add(string firstName, string lastName, string company, string reason)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (ModelState.IsValid)
        {
            // Look up company by name
            var result = await _applicationService.AddApplication(new ApplicationService.AddApplicationQuery()
            {
                UserName = user.UserName,
                SponsorName = company,
                Reason = reason
            });

            if (result == QueryStatus.Success)
            {
                return RedirectToAction("DriverApplication", new {id = user.Id});
                
            }
            ModelState.AddModelError("Company", "Invalid Company.");
        }

        return RedirectToAction("DriverApplication", new {id = user.Id});
    }

    #region Approve/Reject
    
    /*
     * APPROVE APPLICATION
     * function to approve an application
     * Params
     *      int appId - id of the application to be approved
     */
    [HttpPost]
    public async Task<IActionResult> ApproveApplication(int appId, string comment)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        await _applicationService.UpdateApplication(new ApplicationService.UpdateApplicationQuery
        {
            AppId = appId,
            Accepted = true,
            DeciderId = user.Id, 
            Reason = comment
        });

        if (user.UserRole == UserRole.Admin)
        {
            return RedirectToAction("AdminApplication");
        }

        // to return to the same page that called function
        return RedirectToAction("SponsorApplication", new {userName = user.UserName});
    }
    
    /*
     * REJECT APPLICATION
     * function to reject an application
     * Params
     *      int appId - id of the application to be rejected
     */
    [HttpPost]
    public async Task<IActionResult> RejectApplication(int appId, string comment)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        await _applicationService.UpdateApplication(new ApplicationService.UpdateApplicationQuery
        {
            AppId = appId,
            Accepted = false,
            DeciderId = user.Id, 
            Reason = comment
        });
        
        if (user.UserRole == UserRole.Admin)
        {
            return RedirectToAction("AdminApplication");
        }

        // to return to the same page that called function
        return RedirectToAction("SponsorApplication", new {userName = user.UserName});
    }
    #endregion
    
}
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Team22.Web.Models;

namespace Team22.Web.Controllers;

/*
 * This class is going to control functionality from the landing page
 *
 * Once a link is clicked from the index page, it comes to this controller and returns the view specified by the asp-action field
 *
 * prob wanna move to async later
 */


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    #region INDEX/LANDING PAGE
    public IActionResult Index()
    {
        
        return View("Index");
        
        // return View(any parameters for model)
    }
    
    #endregion
    
    
    #region ABOUT
    public IActionResult About()
    {
        return View("About");
    }
    
    #endregion
    
    
    #region CONTACT
    public IActionResult Contact()
    {
        return View("Contact");
    }
    
    #endregion
    
    
    #region FEATURES
    public IActionResult Features()
    {
        return View("Features");
    }
    #endregion


    #region LOGIN

    /*public class Users
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class UserDetails
    {
        public string email { get; set; }

        public Result result { get; set; }
    }

    public class Result
    {
        public bool result { get; set; }
        public string message { get; set; }
    }

    public IActionResult Login()
    {
        return View("Login");
    }*/



    #endregion

    #region CREATE ACCOUNT

    /*public IActionResult CreateAccount()
    {
        return View("CreateAccount");
    }*/
    
    #endregion

    
}
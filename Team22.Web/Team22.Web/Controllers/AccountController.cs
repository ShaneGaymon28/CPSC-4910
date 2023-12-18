using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team22.Web.Contexts;
using Team22.Web.Services;
using Team22.Web.Enums;
using Team22.Web.Interfaces;
using Team22.Web.Models;
using Team22.Web.Utilities;
using Team22.Web.ViewModels;

namespace Team22.Web.Controllers;

/*
 * Controller to handle profile/account pages
 *
 * This controls the currently logged in user's account information
 *
 */

public class AccountController : Controller
{
    private readonly Team22Context _context;
    private readonly UserService _userService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public AccountController(Team22Context context, UserService userService, SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager, IEmailService emailService)
    {
        _context = context;
        _userService = userService;
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
    }
    
    [HttpGet]
    public async Task<IActionResult> DriverAccount()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> SponsorAccount()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> AdminAccount()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(AppUser user)
    {
        /*var userQuery = new UserService.GetUserQuery
        {
            UserName = user.UserName,
            Email = user.Email
        };
        var status = await _userService.GetUserByEmail(userQuery);
        if (status.Status == QueryStatus.Success)
        {
            ModelState.AddModelError("Password", "Failed : Email already in use.");
            return user.UserRole switch
            {
                UserRole.Admin => RedirectToAction("AdminAccount", "Account"),
                UserRole.Sponsor => RedirectToAction("SponsorAccount", "Account"),
                UserRole.Driver => RedirectToAction("DriverAccount", "Account"),
                _ => RedirectToAction("Index", "Home")
            };
        }*/
        
        await _userService.Update(new UserService.UpdateUserQuery
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserRole = user.UserRole,
            UserName = user.UserName
        });

        return user.UserRole switch
        {
            UserRole.Admin => RedirectToAction("AdminAccount", "Account"),
            UserRole.Sponsor => RedirectToAction("SponsorAccount", "Account"),
            UserRole.Driver => RedirectToAction("DriverAccount", "Account"),
            _ => RedirectToAction("Index", "Home")
        };
    }


    public async Task<IActionResult> CreateAccount(string? returnUrl = null)
    {
        RegisterViewModel registerViewModel = new RegisterViewModel();
        registerViewModel.ReturnUrl = returnUrl;
        return View(registerViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(RegisterViewModel registerViewModel, string? returnUrl = null)
    {
        registerViewModel.ReturnUrl = returnUrl;
        returnUrl = returnUrl ?? Url.Content("~/");
        
        var userQuery = new UserService.GetUserQuery
        {
            UserName = registerViewModel.UserName,
            Email = registerViewModel.Email
        };
                
        // direct user to appropriate dashboard
        var status = await _userService.GetUserByEmail(userQuery);
        if (status.Status == QueryStatus.Success)
        {
            ModelState.AddModelError("Password", "Failed : Email already in use.");
            return View(registerViewModel);
        }
        
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                FirstName = registerViewModel.FirstName, LastName = registerViewModel.LastName,
                UserRole = registerViewModel.UserRole,
                Email = registerViewModel.Email, UserName = registerViewModel.UserName,

            };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("Password", result.ToString());
        }

        return View(registerViewModel);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackurl = Url.Action("ResetPassword", "Account", new { userID = user.Id, code = code },
                protocol: HttpContext.Request.Scheme);

            await _emailService.SendEmailAsync(model.Email, "Reset Email Confirmation",
                "Please reset your password by visiting this " +
                "<a href=\"" + callbackurl + "\">link</a>.");
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult ResetPassword(string code=null)
    {
        return code == null ? View("Error") : View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        if(ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if(user == null)
            {
                ModelState.AddModelError("Email", "User not found");
                return View();
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code, resetPasswordViewModel.Password);
            if(result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
        }
        return View(resetPasswordViewModel);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        LoginViewModel loginViewModel = new LoginViewModel();
        loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/");
        
        return View();
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password,
                loginViewModel.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                
                var userQuery = new UserService.GetUserQuery
                {
                    UserName = loginViewModel.UserName
                };
                
                // direct user to appropriate dashboard
                return await _userService.GetUserRole(userQuery) switch
                {
                    UserRole.Admin => RedirectToAction("AdminIndex", "Dashboard"),
                    UserRole.Sponsor => RedirectToAction("SponsorIndex", "Dashboard"),
                    UserRole.Driver => RedirectToAction("Index", "Dashboard"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginViewModel);
            }
        }

        return View(loginViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    
    
    
    // This function is not being used
    /*[HttpGet]
    public async Task<IActionResult> Index(string username)
    {
        return await _userService.GetUser(new UserService.GetUserQuery { UserName = username }) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View(result.Value)
        };
    }*/
}


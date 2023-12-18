using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Team22.Web.Enums;

namespace Team22.Web.ViewModels;

public class RegisterViewModel
{
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; }
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public int Id { get; set; }
    public UserRole UserRole { get; set; }

    // [DataType(DataType.Password)]
    // [Display(Name = "Confirm password")]
    // [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }
}
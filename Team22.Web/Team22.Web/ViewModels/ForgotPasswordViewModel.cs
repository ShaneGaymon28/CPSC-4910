using System.ComponentModel.DataAnnotations;

namespace Team22.Web.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
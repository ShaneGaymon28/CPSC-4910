using Microsoft.AspNetCore.Identity;
using Team22.Web.Enums;

namespace Team22.Web.Models;

public class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public UserRole UserRole { get; set; } = UserRole.Driver; 

    public List<Audit> AuthorEntries { get; set; } = new();

    public List<Audit> SubjectEntries { get; set; } = new();

    public List<Order> Orders { get; set; } = new(); // List of orders the user has made

    // nullable because not every user will have a sponsor
    public List<SponsorUserBridge> Sponsors { get; set; } = new();
    
    public int VerificationId { get; set; }
    public Verification Verification { get; init; } = new();

    public List<Application> Applications { get; set; } = new();
    
    public int? PasswordChangeId { get; set; }
    public PasswordChange? PasswordChange { get; set; }
    
}
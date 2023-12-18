using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Team22.Web.Enums;

namespace Team22.Web.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    public UserRole UserRole { get; set; }

    public int? SponsorId { get; set; } = null;
}
using Microsoft.AspNetCore.Identity;

namespace Team22.Web.Models;

public class SponsorUserBridge
{
    
    public int Id { get; set; }
    public int SponsorId { get; set; }
    
    public Sponsor Sponsor { get; set; }
    
    public int Points { get; set; } = 0;
    
    public string AppUserId { get; set; }
    
    public virtual AppUser AppUser { get; set; }

}
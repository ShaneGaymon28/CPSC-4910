namespace Team22.Web.Models;

public class PasswordChange
{
    public int Id { get; set; }
    
    // whether this password change is optional
    // if an admin resets a user's password, this should be true
    public bool Forced { get; init; } 
    
    public Guid Secret { get; init; } = Guid.NewGuid();
    
    public DateTime Expiration { get; init; } = DateTime.UtcNow.AddDays(7);
}
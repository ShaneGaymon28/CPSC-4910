using Team22.Web.Enums;

namespace Team22.Web.Models;

public class Verification
{
    public int Id { get; set; }

    public Guid Secret { get; set; } = Guid.NewGuid();
    
    public DateTime ExpirationDate { get; init; } = DateTime.UtcNow.AddDays(7);
    
    public VerificationStatus Status { get; set; } = VerificationStatus.EmailPending;
}
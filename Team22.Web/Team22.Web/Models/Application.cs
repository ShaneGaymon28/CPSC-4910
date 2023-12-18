using Team22.Web.Enums;

namespace Team22.Web.Models;

public class Application
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int SponsorId { get; set; }
    public Sponsor Sponsor { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string? Reason { get; set; }
    public string DriverId { get; set; }
}
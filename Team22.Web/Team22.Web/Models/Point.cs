namespace Team22.Web.Models;

public class Point
{
    public int Id { get; set; }
    
    public int CurrentPoints { get; set; } = 0;

    public int DeltaPoints { get; set; } = 0; // change in points

    public AppUser Driver { get; set; } = null!;

    public string DriverId { get; set; } = null!;

    public AppUser Sponsor { get; set; } = null!;

    public string SponsorId { get; set; } = null!; // sponsor user who did the point change

    public DateTime Date { get; set; }

    public string Reason { get; set; } = null!;

    public string SponsorOrg { get; set; } = null!;

}
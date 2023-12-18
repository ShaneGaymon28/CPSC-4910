namespace Team22.Web.Models;

public class Sponsor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public bool AcceptingApps { get; set; } = true;

    public List<SponsorUserBridge> Users { get; set; } = new();

    // 1 Point = $...
    public double PointDollarRatio { get; set; } = 0.01;

    // todo: catalogues for Etsy/eBay/whatever
}
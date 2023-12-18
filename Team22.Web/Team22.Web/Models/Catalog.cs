namespace Team22.Web.Models;

public class Catalog
{
    public int Id { get; set; }
    
    public Sponsor Sponsor { get; set; }
    
    public int SponsorId { get; set; }

    public List<Product> Products { get; set; } = new(); // will get this info from products table


}
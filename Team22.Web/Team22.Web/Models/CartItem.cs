namespace Team22.Web.Models;

public class CartItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    
    public virtual Product Product { get; set; }

    public string AppUserId { get; set; }
    
    public virtual AppUser AppUser { get; set; }
    
}
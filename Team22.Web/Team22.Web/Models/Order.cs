using Team22.Web.Enums;

namespace Team22.Web.Models;

public class Order
{
    public int Id { get; set; }
    
    public int UserId { get; set; } // ID of user who made order
    public User User { get; set; }
    
    public DateTime TimeStamp { get; set; }

    public List<Product> Products { get; set; } = new(); // List of products in order

    public OrderStatus OrderStatus { get; set; } = OrderStatus.NotPlaced;
}
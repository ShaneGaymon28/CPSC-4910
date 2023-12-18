using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

public class CartService
{
    private readonly Team22Context _context;
    
    public CartService(Team22Context context)
    {
        _context = context;
    }

    public async Task<QueryResult<List<CartItem>>> GetCartByUserId(string userId)
    {
        var items = await _context.CartItems.Where(c => c.AppUserId == userId)
            .Include(c => c.Product)
            .ToListAsync();
        
        // no cart items found
        if (items.Count < 1)
        {
            return QueryResult<List<CartItem>>.NotFound();
        }
        
        return QueryResult<List<CartItem>>.Success(items);
    }

    public async Task<QueryStatus> AddToCart(string userId, int productId)
    {
        var newItem = new CartItem
        {
            AppUserId = userId,
            ProductId = productId
        };

        _context.CartItems.Add(newItem);
        await _context.SaveChangesAsync();

        return QueryStatus.Success;
    }

    public async Task<QueryStatus> RemoveFromCart(int cartItemId, string userId)
    {
        var cartItem = await _context.CartItems.Where(c => c.AppUserId == userId)
            .FirstOrDefaultAsync(c => c.Id == cartItemId);
        
        if (cartItem is null)
        {
            return QueryStatus.NotFound;
        }

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        
        return QueryStatus.Success;
    }
    
}
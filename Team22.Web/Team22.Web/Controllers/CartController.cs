using System.Net;
using Microsoft.AspNetCore.Mvc;
using Team22.Web.Models;
using Team22.Web.Services;
using Microsoft.AspNetCore.Identity;
using Team22.Web.Enums;

namespace Team22.Web.Controllers;

public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly UserManager<AppUser> _userManager;
    
    public CartController(CartService cartService, UserManager<AppUser> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    /**
     * DRIVER VIEW CART
     * gets the items in a driver's cart and returns them to view 
     */
    [HttpGet]    
    public async Task<IActionResult> ViewCart()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);

        return await _cartService.GetCartByUserId(user.Id) switch
        {
            { Status: QueryStatus.NotFound } => View(new List<CartItem>()), // i.e. empty cart
            { Status: QueryStatus.Success } result => View(result.Value)
        };

    }
    
    /**
     * ADD TO CART
     * adds a product to the driver's cart
     */
    public async Task<IActionResult> AddToCart(int productId)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        await _cartService.AddToCart(user.Id, productId);
        
        return Redirect(Request.Headers["Referer"].ToString());
    }

    /**
     * REMOVE FROM CART
     * removes an item from the driver's cart
     */
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        await _cartService.RemoveFromCart(cartItemId, user.Id);
        
        return Redirect(Request.Headers["Referer"].ToString());
    }
    
}
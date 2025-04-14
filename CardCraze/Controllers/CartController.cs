//Author: Adrian
using CardCraze.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCraze.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CardCrazeDbContext _context;

        public CartController(CardCrazeDbContext context)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7283/")
            };
            _context = context;
        }

        //View user's cart
        [HttpGet]
        public async Task<IActionResult> MyCart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Card)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            return View(cartItems);
        }
        
        //Update amount of cards within the cart
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                _context.CartItems.Update(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyCart");
        }

        //Remove the Card from Cart if user no longer wants it
        [HttpPost]
        public async Task<IActionResult> Remove(int itemId)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.CartID == itemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyCart");

        }

        //Send user to confirmation page once order is "placed" 
        [HttpPost]
        public IActionResult Confirmation(string action)
        {
            ViewBag.ActionMessage = action;
            return View("Confirmation");
        }

    }
}

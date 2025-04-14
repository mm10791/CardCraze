//Author of class: Adrian

using CardCraze.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCraze.Controllers
{
    public class WishlistController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CardCrazeDbContext _context;

        public WishlistController(CardCrazeDbContext context)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7283/")
            };
            _context = context;
        }
        //View entire Wishlist list and display each card based on user
        [HttpGet]
        public async Task<IActionResult> MyWishlist()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var wishlist = await _context.Wishlists
                .Include(w => w.Card)
                .Where(w => w.UserID == userId)
                .ToListAsync();

            return View(wishlist);
        }
        // Remove card from wishlist
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var item = await _context.Wishlists.FindAsync(id);
            if (item != null && item.UserID == userId)
            {
                _context.Wishlists.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyWishlist");
        }
    }
}

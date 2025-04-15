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
        //[HttpPost]
        //public IActionResult Confirmation(string action)
        //{
        //    ViewBag.ActionMessage = action;
        //    return View("Confirmation");
        //}

        //Tristan Edit Below for order history Compatability
        [HttpPost]
        public async Task<IActionResult> Confirmation()
        {
            //get user id from session - redirect if not found
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            //get all cart items that user has alrdy
            var cartItems = await _context.CartItems
                .Include(c => c.Card)
                .Where(c => c.UserID == userId)
                .ToListAsync();
            //in case cart is empty
            if (!cartItems.Any())
            {
                ViewBag.Message = "Your cart is empty!";
                return RedirectToAction("MyCart");
            }
            //each unique item gets a new order history row (new table in db)
            foreach (var item in cartItems)
            {
                var orderEntry = new OrderHistory
                {
                    UserID = userId.Value,
                    CardID = item.CardID,
                    Quantity = item.Quantity,
                    OrderDate = DateTime.Now
                };
                
                _context.OrderHistories.Add(orderEntry);
            }
            //remove the cart items once done
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return View("Confirmation");
        }

    }
}

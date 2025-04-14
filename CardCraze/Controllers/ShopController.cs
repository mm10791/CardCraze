//Author: Max

using CardCraze.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace CardCraze.Controllers
{
    public class ShopController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CardCrazeDbContext _context;

        public ShopController(CardCrazeDbContext context, HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri("https://localhost:7283/") };
            _context = context;
        }

        public IActionResult Confirmation(string action)
        {
            ViewBag.ActionMessage = action;
            return View();
        }

        [HttpGet("Shop/BrowseCards")]
        public async Task<IActionResult> BrowseCards(string rarity, decimal? minPrice, decimal? maxPrice)
        {
            var response = await _httpClient.GetAsync("api/card");
            var cards = new List<Card>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                cards = JsonConvert.DeserializeObject<List<Card>>(json);
            }

            //apply filters
            if (!string.IsNullOrEmpty(rarity))
            {
                cards = cards.Where(c => c.Rarity == rarity).ToList();
            }

            if (minPrice.HasValue)
            {
                cards = cards.Where(c => c.Price >= minPrice.Value).ToList();
            }

            if (maxPrice.HasValue)
            {
                cards = cards.Where(c => c.Price <= maxPrice.Value).ToList();
            }

            return View("BrowseCards", cards);
        }



        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int cardId)
        {
            //get the userId from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItem = new Wishlist
            {
                CardID = cardId,
                UserID = userId.Value
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Confirmation", new { action = "added to wishlist" });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //checking if the card already exists in the cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CardID == cardId && c.UserID == userId.Value);

            if (existingCartItem != null)
            {
                //if the item is already in the cart then increase the quantity
                existingCartItem.Quantity += 1;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                //otherwise add a new cart item with 1
                var newCartItem = new CartItem
                {
                    CardID = cardId,
                    UserID = userId.Value,
                    Quantity = 1
                };

                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Confirmation", new { action = "added to cart" });
        }

        public IActionResult AllCards()
        {
            var allCards = _context.Cards.ToList();
            return View(allCards);
        }

    }
}


//Author of class: Max

using CardCraze.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCraze.Controllers
{
    public class CardController : Controller
    {
        private readonly CardCrazeDbContext _context;

        public CardController(CardCrazeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Browse(string rarity, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Cards.AsQueryable();

            if (!string.IsNullOrEmpty(rarity))
                query = query.Where(c => c.Rarity == rarity);

            if (minPrice.HasValue)
                query = query.Where(c => c.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.Price <= maxPrice.Value);

            var cards = await query.ToListAsync();
            return View("BrowseCards", cards);

        }

    }
}

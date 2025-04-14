//Author: Adrian
using CardCrazeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCrazeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly CardCrazeDbContext _dbContext;
        public WishlistController(CardCrazeDbContext _context)
        {
            _dbContext = _context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlist(int id)
        {
            return await _dbContext.Wishlists.Include(w => w.Card)
                            .Where(w => w.UserID == id)
                            .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Wishlist>> AddToWishlist(Wishlist item)
        {
            _dbContext.Wishlists.Add(item);
            await _dbContext.SaveChangesAsync();
            return Ok(GetWishlist(item.UserID));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveFromList(int id)
        {
            var item = await _dbContext.Wishlists.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            _dbContext.Wishlists.Remove(item);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}

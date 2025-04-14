//Author: Adrian
using CardCrazeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCrazeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CardCrazeDbContext _context;

        public CartController(CardCrazeDbContext context)
        {
            _context = context;
        }

        //GET: api/Cart/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCart(int id)
        {
            return await _context.CartItems
                .Include(c => c.Card)
                .Where(c => c.UserID == id)
                .ToListAsync();
        }

        //POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart(CartItem item)
        {
            var existing = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserID == item.UserID && c.CardID == item.CardID);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                _context.CartItems.Add(item);
            }
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        //PUT: api/Cart
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] int quantity)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null) return NotFound();
            if (quantity < 1)
            {
                return BadRequest();
            }

            item.Quantity = quantity;
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        //DELETE: api/Cart
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _context.CartItems.FindAsync(id);

            if (item == null) return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

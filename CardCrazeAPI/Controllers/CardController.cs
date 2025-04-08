//Author of class: Max

using CardCrazeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCrazeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly CardCrazeDbContext _context;

        public CardController(CardCrazeDbContext context)
        {
            _context = context;
        }

        // GET: api/cards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Card>>> GetCards()
        {
            try
            {
                return await _context.Cards.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving cards: {ex.Message}");
            }
        }

        // GET: api/cards/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(int id)
        {
            try
            {
                var card = await _context.Cards.FindAsync(id);
                if (card == null) return NotFound();
                return card;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving card: {ex.Message}");
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<Card>> PostCard(Card card)
        {
            try
            {
                _context.Cards.Add(card);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCard), new { id = card.CardID }, card);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding card: {ex.Message}");
            }
        }

        // PUT: api/cards/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(int id, Card card)
        {
            if (id != card.CardID)
                return BadRequest("Card ID mismatch");

            var existingCard = await _context.Cards.FindAsync(id);
            if (existingCard == null)
                return NotFound();

            existingCard.Name = card.Name;
            existingCard.Rarity = card.Rarity;
            existingCard.Price = card.Price;
            existingCard.ImageURL = card.ImageURL;
 
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error updating CARD");
            }

            return Ok("Card details updated successfully");
        }


        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound();
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return Ok("Card deleted successfully");
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.CardID == id);
        }
    }
}

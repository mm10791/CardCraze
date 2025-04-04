using Microsoft.EntityFrameworkCore;

namespace CardCraze.Models
{
	public class CardCrazeDbContext : DbContext
	{
		public CardCrazeDbContext(DbContextOptions<CardCrazeDbContext> options) : base(options) { }

		public DbSet<Card> Cards { get; set; }

		public DbSet<Wishlist> Wishlists { get; set; }
	}
}

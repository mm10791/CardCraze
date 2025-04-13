//Author of class: Max

using Microsoft.EntityFrameworkCore;

namespace CardCrazeAPI.Models
{
    public class FakeCardData
    {
        public static void LoadData(IApplicationBuilder app)
        {
            CardCrazeDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<CardCrazeDbContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }

                //seed pokemon cards
                if (!context.Cards.Any())
                {
                    context.Cards.AddRange(
                        new Card { Name = "Pikachu", Rarity = "Legendary", Price = 50.00m, ImageURL = "/Images/pikachuCard.jpg" },
                        new Card { Name = "Charizard", Rarity = "Rare", Price = 100.00m, ImageURL = "/Images/charizardCard.jpg" },
                        new Card { Name = "Bulbasaur", Rarity = "Common", Price = 5.00m, ImageURL = "/Images/bulbasaurCard.jpg" },
                        new Card { Name = "Squirtle", Rarity = "Common", Price = 5.00m, ImageURL = "/Images/squirtleCard.jpg" },
                        new Card { Name = "Jigglypuff", Rarity = "Rare", Price = 20.00m, ImageURL = "/Images/jigglypuffCard.png" },
                        new Card { Name = "Mewtwo", Rarity = "Legendary", Price = 150.00m, ImageURL = "/Images/mewtwoCard.jpg" },
                        new Card { Name = "Zekrom", Rarity = "Legendary", Price = 120.00m, ImageURL = "/Images/zekromCard.jpg" },
                        new Card { Name = "Giratina", Rarity = "Rare", Price = 30.00m, ImageURL = "/Images/giratinaCard.jpg" },
                        new Card { Name = "Growlithe", Rarity = "Common", Price = 3.00m, ImageURL = "/Images/growlitheCard.png" },
                        new Card { Name = "Gengar", Rarity = "Common", Price = 12.00m, ImageURL = "/Images/gengarCard.png" },
                        new Card { Name = "Mew", Rarity = "Rare", Price = 75.00m, ImageURL = "/Images/mewCard.png" },
                        new Card { Name = "Greninja", Rarity = "Legendary", Price = 135.00m, ImageURL = "/Images/greninjaCard.png" }
                    );
                    context.SaveChanges();
                }
        }
    }
}

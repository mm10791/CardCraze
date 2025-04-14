using NUnit.Framework;
using CardCraze.Controllers;
using CardCraze.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Moq.Protected;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace CardCraze.Tests
{
    //Test Author: Max
    public class CardControllerTests
    {
        private CardCrazeDbContext _context;
        private CardController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CardCrazeDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new CardCrazeDbContext(options);

            // Seed some test data
            _context.Cards.AddRange(new List<Card>
            {
                new Card { Name = "Snorlax", Rarity = "Common", Price = 5.00m, ImageURL = "/Images/snorlaxCard.jpg" },
                new Card { Name = "Alakazam", Rarity = "Rare", Price = 25.00m, ImageURL = "/Images/AlakazamCard.jpg" }
            });
            _context.SaveChanges();

            _controller = new CardController(_context);
        }


        [Test]
        public async Task Browse_ReturnsFilteredCards_ByRarity()
        {
            var result = await _controller.Browse("Rare", null, null) as ViewResult;
            var cards = result?.Model as IEnumerable<Card>;

            Assert.That(cards, Is.Not.Null);
            Assert.That(cards.Count(), Is.EqualTo(1));
            Assert.That(cards.First().Name, Is.EqualTo("Alakazam"));
        }

        [TearDown]
        public void Cleanup()
        {
            _controller?.Dispose();
            _context.Dispose();
        }
    }

    //Test Author: Max
    public class ShopControllerTests
    {
        private CardCrazeDbContext _context;
        private ShopController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CardCrazeDbContext>()
                .UseInMemoryDatabase("TestDb_Shop")
                .Options;

            _context = new CardCrazeDbContext(options);
        }

        [Test]
        public async Task BrowseCards_ReturnsFilteredResults_ByRarity()
        {
            //test cards
            var testCards = new List<Card>
            {
                new Card { Name = "Charizard", Rarity = "Rare", Price = 100.00m, ImageURL = "/Images/charizardCard.jpg" },
                new Card { Name = "Mewtwo", Rarity = "Legendary", Price = 150.00m, ImageURL = "/Images/mewtwoCard.jpg" },
            };

            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(testCards), Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://localhost:7283/")
            };

            _controller = new ShopController(_context, client);

            //act
            var result = await _controller.BrowseCards("Legendary", null, null) as ViewResult;
            var cards = result?.Model as List<Card>;

            //assert
            Assert.That(result, Is.Not.Null);
            Assert.That(cards, Is.Not.Null );
            Assert.That(cards.Count, Is.EqualTo(1));
            Assert.That(cards.First().Name, Is.EqualTo("Mewtwo"));
        }

        
        [Test]
        public async Task BrowseCards_ReturnsFilteredResults_ByPriceRange()
        {
            var testCards = new List<Card>
            {
                new Card { Name = "Bulbasaur", Rarity = "Common", Price = 10.00m, ImageURL = "/Images/bulbasaurCard.jpg" },
                new Card { Name = "Gengar", Rarity = "Rare", Price = 50.00m, ImageURL = "/Images/gengarCard.jpg" },
                new Card { Name = "Rayquaza", Rarity = "Legendary", Price = 150.00m, ImageURL = "/Images/rayquazaCard.jpg" },
            };

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(testCards), Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://localhost:7283/")
            };

            _controller = new ShopController(_context, client);

            //act: filter between $20 and $100
            var result = await _controller.BrowseCards(null, 20.00m, 100.00m) as ViewResult;
            var cards = result?.Model as IEnumerable<Card>;

            //assert
            Assert.That(cards, Is.Not.Null);
            Assert.That(cards.Count(), Is.EqualTo(1));
            Assert.That(cards.First().Name, Is.EqualTo("Gengar"));
        }

        [TearDown]
        public void Cleanup()
        {
            _controller?.Dispose();
            _context.Dispose();
        }
    }

}

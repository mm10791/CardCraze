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
using Microsoft.AspNetCore.Http;

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
            Assert.That(cards, Is.Not.Null);
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
    //Author: Adrian
    public class CartControllerTests
    {
        private CardCrazeDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CardCrazeDbContext>()
                .UseInMemoryDatabase("TestDB")
                .Options;

            _context = new CardCrazeDbContext(options);
        }
        [Test]
        public async Task MyCart_RedirectsToLogin_WhenUserIsNotLoggedIn()
        {
            var controller = new CartController(_context)
            {
                ControllerContext = CreateControllerContextWithSession(null)
            };

            var result = await controller.MyCart();

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Login"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Account"));
        }

        [Test]
        public async Task UpdateQuantity_ChangesQuantity()
        {
            var item = new CartItem { CartID = 1, UserID = 1, Quantity = 1 };
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();

            var controller = new CartController(_context);

            var result = await controller.UpdateQuantity(item.CartID, 5);

            var updated = await _context.CartItems.FindAsync(item.CartID);
            Assert.That(updated.Quantity, Is.EqualTo(5));

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("MyCart"));
        }

        [Test]
        public async Task Remove_DeletesItem()
        {
            var item = new CartItem { CartID = 1, UserID = 1, Quantity = 1 };
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();

            var controller = new CartController(_context);

            var result = await controller.Remove(item.CartID);

            var deleted = await _context.CartItems.FindAsync(item.CartID);
            Assert.That(deleted, Is.Null);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("MyCart"));
        }





        // Created to simulate a fake session
        private ControllerContext CreateControllerContextWithSession(int? userId)
        {
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();

            if (userId.HasValue)
                session.SetInt32("UserId", userId.Value);

            httpContext.Session = session;

            return new ControllerContext
            {
                HttpContext = httpContext
            };
        }
        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
        //Test session class to product required session.
        private class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _sessionStorage = new();

            public bool IsAvailable => true;
            public string Id => Guid.NewGuid().ToString();
            public IEnumerable<string> Keys => _sessionStorage.Keys;

            public void Clear() => _sessionStorage.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public void Remove(string key) => _sessionStorage.Remove(key);
            public void Set(string key, byte[] value) => _sessionStorage[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
        }
    }



    //Author: Tristan

    public class AccountControllerTests
    {
        private CardCrazeDbContext _context;
        private HttpClient GetFakeHttpClient()
        {
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("") 
                });

            return new HttpClient(mockHandler.Object);
        }

        [Test]
        public async Task SignUp_Fails_WhenPasswordsDoNotMatch()
        {
            var fakeClient = GetFakeHttpClient();
            var controller = new AccountController(_context);

            var user = new User
            {
                Email = "test@example.com",
                Password = "test",
                ConfirmPassword = "testttt"
            };

            var result = await controller.SignUp(user);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(controller.ViewBag.Message, Is.EqualTo("Passwords dont match"));
        }

        [Test]
        public async Task Login_ReturnsView_WhenEmailIsMissing() //returning the view when user doesnt put in email
        {
            var controller = new AccountController(_context);

            controller.ModelState.AddModelError("Email", "Email is required");

            var loginUser = new LoginUser
            {
                Email = "", // user forgets to put in email
                Password = "password"
            };

            var result = await controller.Login(loginUser);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(controller.ViewBag.ErrorMessage, Is.EqualTo("Validation failed."));
        }

        [Test]
        public async Task Login_ReturnsView_WhenPasswordIsMissing() //returning the view when user doesnt put in password
        {
            var controller = new AccountController(_context);

            controller.ModelState.AddModelError("Password", "Password is required");

            var loginUser = new LoginUser
            {
                Email = "test@example.com",
                Password = "" // now user forgets to put in password
            };

            var result = await controller.Login(loginUser);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(controller.ViewBag.ErrorMessage, Is.EqualTo("Validation failed."));
        }





    }
}

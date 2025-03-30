using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CardCraze.Models;
using Microsoft.EntityFrameworkCore;

namespace CardCraze.Controllers
{
    public class HomeController : Controller
    {
        private readonly CardCrazeDbContext _context;

        public HomeController(CardCrazeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var featuredCards = _context.Cards.Take(6).ToList();

            var model = new FeaturedCards
            {
                Cards = featuredCards
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

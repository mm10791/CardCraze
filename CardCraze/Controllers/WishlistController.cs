//Author of class: Max

using Microsoft.AspNetCore.Mvc;

namespace CardCraze.Controllers
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

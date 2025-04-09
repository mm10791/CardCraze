using Microsoft.AspNetCore.Mvc;

namespace CardCraze.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

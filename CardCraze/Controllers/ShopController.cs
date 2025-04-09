using Microsoft.AspNetCore.Mvc;

namespace CardCraze.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

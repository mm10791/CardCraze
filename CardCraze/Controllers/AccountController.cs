//Author of class: Tristan 

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CardCraze.Controllers
{
    public class AccountController : Controller
    {

        private readonly HttpClient _httpClient;

        public AccountController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7283/")
            };
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(User model)
        {

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/user", content);

            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Message = "Sign up failed";
            return View();
           
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

    }
}

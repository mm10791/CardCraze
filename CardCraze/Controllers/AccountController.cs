//Author of class: Tristan 

using CardCraze.Models;
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

        [HttpPost]
        public async Task<IActionResult> SignUp(User model)
        {

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/user", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Message = "Sign up failed";
            return View();

        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Validation failed.";
                return View(model);
            }

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/user/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            HttpContext.Session.SetInt32("UserId", notification.UserId);
            return RedirectToAction("Dashboard", new { userId = notification.UserId });
        }

        public async Task<IActionResult> Dashboard(int userId)
        {
            var response = await _httpClient.GetAsync($"api/user/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            return View(user);
        }

    }
}

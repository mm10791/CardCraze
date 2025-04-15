//Author of class: Tristan 

using CardCraze.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;



namespace CardCraze.Controllers
{
    public class AccountController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly CardCrazeDbContext _context;

        public AccountController(CardCrazeDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7283/")
            };
           
        }

        //get signup page
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        //entering details in signup page
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

        //getting login page
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        //entering details in login
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


        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var response = await _httpClient.GetAsync($"api/user/{userId}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            return View(user);
        }


        //getting full details of user for form submission (requires names as well)
        [HttpGet]
        public async Task<IActionResult> UpdateProfile(int userId)
        {
            var response = await _httpClient.GetAsync($"api/user/{userId}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(json);

            return View(user);
        }

        //updating user account
        [HttpPost]
        public async Task<IActionResult>  UpdateProfile(User updatedUser)
        {
            Console.WriteLine("POST received: " + updatedUser.Email);
            if (!ModelState.IsValid)
            {
                return View(updatedUser);
            }

            var json = JsonConvert.SerializeObject(updatedUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/user/{updatedUser.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Profiled updated";
                return RedirectToAction("Dashboard", new {userId = updatedUser.Id});
            
            }
            ViewBag.ErrorMessage = "Failed to update profile.";
            return View(updatedUser);

        }

        //display order history
        public async Task<IActionResult> OrderHistory()
        {
            //get id from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            //search orders placed on these card by various markers, date, orders placed by user, and necessary card deets
            var orders = await _context.OrderHistories
                .Include(o => o.Card)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


    }
}

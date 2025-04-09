//Author of class: Tristan 

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CardCraze.Models;
using Microsoft.EntityFrameworkCore;
namespace CardCraze.Controllers


{
    public class AccountController : Controller
    {

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
        public IActionResult SignUp(User user)
        {
            if(ModelState.IsValid)
            {
                return RedirectToAction("Success");
            }
            return View(user);
        }

        public IActionResult Success()
        {
            return Content("New Account Made, Welcome!");
        }

    }
}

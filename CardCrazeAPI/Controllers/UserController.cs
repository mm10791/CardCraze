﻿//author: Tristan
using CardCrazeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CardCrazeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CardCrazeDbContext _context;
        public UserController(CardCrazeDbContext context)
        {
            _context = context;
        }

        //for signing up
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
        //used for testing purposes in swagger
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {


            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();
                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}" );
            }

        }


        //for logging in
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == request.Email.ToLower() &&
                    u.Password == request.Password);

            if (user == null)
                return NotFound(new { message = "Invalid email or password" });

            return Ok(new { message = "Login succeeded", userId = user.Id });
        }

        //for editing account
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            //updating email and password for now
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        //for getting user card collection
        [HttpGet("{userId}/collection")]
        public async Task<IActionResult> GetUserCardCollection(int userId)
        {
            var collection = await _context.OrderHistories
                .Include(o => o.Card)
                .Where(o => o.UserID == userId)
                .ToListAsync();

            return Ok(collection);
        }



    }
}

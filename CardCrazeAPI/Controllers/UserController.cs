//author: tristan
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

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
        //testing
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


        //logging in
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

    }
}

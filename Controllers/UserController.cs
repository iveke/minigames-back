using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using Microsoft.AspNetCore.Authorization;
using MiniGame.Helpers;
using System.Security.Claims;

namespace MyBackend.Controllers
{
    [Route("/user")]
    [ApiController]


    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly JwtHelper _jwtHelper;


        public UserController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("getInfo/{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // var role = _jwtHelper.GetUsernameFromToken();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost("create")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserData updateData, [CurrentUser] User currentUser)
        {
            if (currentUser == null) return Unauthorized();

            _context.Entry(updateData).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"THIS ROLE: {role}");
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    public class UpdateUserData
    {
        public int? age;
        public string? username;
    }
}

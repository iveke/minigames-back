using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using Microsoft.AspNetCore.Authorization;
using MiniGame.Helpers;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MiniGame.Services;

namespace MyBackend.Controllers
{
    [Route("/user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper;


        public UserController(UserService userService, JwtHelper jwtHelper, AppDbContext context)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
            _context = context;
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpGet("getInfo/{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // [HttpPost("create")]
        // public async Task<ActionResult<User>> CreateUser(CreateModel data)
        // {
        //     var user = await _userService.CreateUserAsync(data);
        //     return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        //     // user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        //     // _context.Users.Add(user);
        //     // await _context.SaveChangesAsync();
        //     // return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        // }

        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateModel updateData, [CurrentUser] User currentUser)
        {
            if (currentUser == null) return Unauthorized();
            User result = await _userService.UpdateUserAsync(currentUser, updateData);
            if (result == null) return NotFound();

            return Ok(result);
            // _context.Entry(updateData).State = EntityState.Modified;
            // await _context.SaveChangesAsync();

            // return NoContent();
        }

        [HttpPost("confirmEmail/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail(int id)
        {
            try
            {
                var result = await _userService.ConfirmEmailAsync(id);
                if (!result) return NotFound();
                return Ok(new { message = "Email confirmed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            try
            {
                var result = await _userService.DeleteUserAsync(id, role);
                if (!result) return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            return NoContent();
        }
    }
}

public class UpdateModel
{
    // [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string? username { get; set; }
    // [Phone(ErrorMessage = "Invalid phone number format")]
    public string? phone { get; set; }
    public int? Age { get; set; }
    public string? Country { get; set; }
}


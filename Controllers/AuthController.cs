using Microsoft.AspNetCore.Mvc;
using MiniGame.Data;
using MiniGame.Helpers;
using MiniGame.Models;
using MiniGame.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Drawing;

namespace MyBackend.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly UserService _userService;


        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, AuthService authService, JwtHelper jwtHelper, UserService userService)
        {
            _context = context;
            _authService = authService;
            _jwtHelper = jwtHelper;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateModel data)
        {
            User user = await _userService.CreateUserAsync(data);
            var token = _jwtHelper.GenerateJwtToken(user.Username, user.Email, user.Password, user.Role, user.Id);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel data)
        {
            var dbUser = _context.Users.FirstOrDefault(u => u.Email == data.email);
            if (dbUser == null)
            {
                return Unauthorized();
            }

            if (!BCrypt.Net.BCrypt.Verify(data.password, dbUser.Password))
            {
                return BadRequest(new { message = "InvalidPassword" });
            }
            var token = _jwtHelper.GenerateJwtToken(dbUser.Username, dbUser.Email, dbUser.Password, dbUser.Role, dbUser.Id);
            return Ok(new { token });
        }
        public class LoginModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string password { get; set; }
        }

    }
}

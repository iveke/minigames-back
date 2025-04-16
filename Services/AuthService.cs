using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniGame.Data;
using MiniGame.Models;

namespace MiniGame.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;


        public AuthService(IConfiguration config, AppDbContext context, EmailService emailService)
        {
            _config = config;
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> SendCodeToEmail(User user)
        {
            try
            {
                var code = new Random().Next(10000, 99999).ToString();
                user.EmailVerificationCode = code;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await this._emailService.SendEmailAsync(user.Email, user.EmailVerificationCode);
                return true;

            }
            catch
            {
                return false;
            }
        }

        // public string GenerateToken(string username, RoleEnum role, string email, string password)
        // {
        //     var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        //     var claims = new[] { new Claim(ClaimTypes.Name, username) };
        //     var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        //     var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: credentials);
        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }
    }
}
